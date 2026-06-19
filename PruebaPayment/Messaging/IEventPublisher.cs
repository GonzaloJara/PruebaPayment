namespace PruebaPayment.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string exchange, string routingKey, TEvent message, CancellationToken ct);
}
