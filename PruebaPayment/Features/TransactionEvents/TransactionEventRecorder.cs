using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.TransactionEvents;

public interface ITransactionEventRecorder
{
    Task RecordAsync(
        Guid transactionId,
        TransactionEventType eventType,
        TransactionEventLevel level,
        string message,
        string? details = null,
        CancellationToken ct = default);
}
public class TransactionEventRecorder(PaymentsDbContext dbContext, ILogger<TransactionEventRecorder> logger)
    : ITransactionEventRecorder
{
    public async Task RecordAsync(
        Guid transactionId,
        TransactionEventType eventType,
        TransactionEventLevel level,
        string message,
        string? details = null,
        CancellationToken ct = default)
    {
        try
        {
            await dbContext.TransactionEvents.AddAsync(new TransactionEvent
            {
                TransactionId = transactionId,
                EventType = eventType,
                Level = level,
                Message = message,
                Details = details
            }, ct);

            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            // Recording an event must never break the flow it is observing.
            logger.LogError(ex, "Failed to record transaction event {EventType} for transaction {TransactionId}", eventType, transactionId);
        }
    }
}
