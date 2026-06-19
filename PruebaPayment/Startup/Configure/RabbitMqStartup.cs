using PruebaPayment.Messaging;
using RabbitMQ.Client;

namespace PruebaPayment.Startup.Configure;

public static class RabbitMqStartup
{
    public const string PaymentEventsExchange = "payment.events";
    public const string PaymentCreatedRoutingKey = "payment.created";
    public const string PaymentCreatedQueue = "payment.created.queue";
    public const string PaymentCreatedDeadLetterQueue = "payment.created.queue.dlq";
    public const string RetryCountHeader = "x-retry-count";
    public const int MaxDeliveryAttempts = 3;

    internal static void AddRabbitMq(WebApplicationBuilder builder)
    {
        builder.AddRabbitMQClient(connectionName: "payment-mq",
            options =>
            {
                options.DisableAutoActivation = false;
            });

        builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
    }

    internal static async Task UseRabbitMqAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: PaymentEventsExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: PaymentCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: PaymentCreatedQueue,
            exchange: PaymentEventsExchange,
            routingKey: PaymentCreatedRoutingKey);

        // Holds messages that failed processing after MaxDeliveryAttempts, for manual inspection/replay
        await channel.QueueDeclareAsync(
            queue: PaymentCreatedDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);
    }
}
