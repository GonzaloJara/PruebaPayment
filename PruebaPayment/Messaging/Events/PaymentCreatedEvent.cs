using PruebaPayment.DB.Models;

namespace PruebaPayment.Messaging.Events;

public record PaymentCreatedEvent(
    Guid TransactionId,
    Guid MerchantId,
    decimal Amount,
    string Currency,
    PaymentRequestStatus Status,
    DateTime CreatedAt)
{
    public static PaymentCreatedEvent From(PaymentRequest entity)
        => new(entity.TransactionId, entity.MerchantId, entity.Amount, entity.Currency, entity.Status, entity.CreatedAt);
}
