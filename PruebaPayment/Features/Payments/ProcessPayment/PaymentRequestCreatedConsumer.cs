using System.Text.Json;
using PruebaPayment.Messaging.Events;
using PruebaPayment.Startup.Configure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PruebaPayment.Features.Payments.ProcessPayment;

public class PaymentRequestCreatedConsumer(
    IConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<PaymentRequestCreatedConsumer> logger)
    : BackgroundService
{
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: RabbitMqStartup.PaymentCreatedQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProcessPaymentRequestService>();

        try
        {
            var @event = JsonSerializer.Deserialize<PaymentCreatedEvent>(args.Body.Span)
                ?? throw new InvalidOperationException("Could not deserialize PaymentCreatedEvent");

            logger.LogInformation("Consumed PaymentCreatedEvent for transaction {TransactionId}", @event.TransactionId);

            await service.ProcessAsync(@event.TransactionId, CancellationToken.None);

            await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false);

            logger.LogInformation("Acked PaymentCreatedEvent for transaction {TransactionId}", @event.TransactionId);
        }
        catch (Exception ex)
        {
            await HandleFailureAsync(args, ex);
        }
    }

    private async Task HandleFailureAsync(BasicDeliverEventArgs args, Exception ex)
    {
        int attempt = GetRetryCount(args.BasicProperties) + 1;

        if (attempt >= RabbitMqStartup.MaxDeliveryAttempts)
        {
            logger.LogError(ex, "Failed to process PaymentCreatedEvent after {Attempt} attempt(s), moving to dead-letter queue", attempt);
            await _channel!.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: RabbitMqStartup.PaymentCreatedDeadLetterQueue,
                mandatory: false,
                basicProperties: new BasicProperties(args.BasicProperties),
                body: args.Body);
        }
        else
        {
            logger.LogWarning(ex, "Failed to process PaymentCreatedEvent (attempt {Attempt}/{MaxAttempts}), requeuing", attempt, RabbitMqStartup.MaxDeliveryAttempts);

            var properties = new BasicProperties(args.BasicProperties)
            {
                Headers = new Dictionary<string, object?>(args.BasicProperties.Headers ?? new Dictionary<string, object?>())
                {
                    [RabbitMqStartup.RetryCountHeader] = attempt
                }
            };

            await _channel!.BasicPublishAsync(
                exchange: RabbitMqStartup.PaymentEventsExchange,
                routingKey: RabbitMqStartup.PaymentCreatedRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: args.Body);
        }

        // The original delivery is always acked: it has either been requeued via a fresh publish
        // (with the incremented retry header) or sent to the dead-letter queue.
        await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
    }

    private static int GetRetryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is not null && properties.Headers.TryGetValue(RabbitMqStartup.RetryCountHeader, out var value) && value is not null)
        {
            return Convert.ToInt32(value);
        }

        return 0;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
