using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.TransactionEvents.GetTransactionEvents;

public record TransactionEventViewModel(
    long Id,
    Guid TransactionId,
    string EventType,
    string Level,
    string Message,
    string? Details,
    DateTime CreatedAt)
{
    public static TransactionEventViewModel From(TransactionEvent entity)
        => new(entity.Id, entity.TransactionId, entity.EventType.ToString(), entity.Level.ToString(), entity.Message, entity.Details, entity.CreatedAt);
}
