using System.Text.Json;
using RabbitMQ.Client;

namespace PruebaPayment.Messaging;

public class RabbitMqEventPublisher(IConnection connection, ILogger<RabbitMqEventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(string exchange, string routingKey, TEvent message, CancellationToken ct)
    {
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: ct);

        logger.LogInformation(
            "Published {EventType} message to exchange {Exchange} with routing key {RoutingKey}",
            typeof(TEvent).Name, exchange, routingKey);
    }
}
