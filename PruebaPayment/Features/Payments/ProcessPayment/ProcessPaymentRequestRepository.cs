using PruebaPayment.DB;
using PruebaPayment.DB.Models;

namespace PruebaPayment.Features.Payments.ProcessPayment;

public interface IProcessPaymentRequestRepository
{
    Task<PaymentRequest?> GetByIdAsync(Guid transactionId, CancellationToken ct);
    Task UpdateStatusAsync(PaymentRequest entity, PaymentRequestStatus status, CancellationToken ct);
}

public class ProcessPaymentRequestRepository(PaymentsDbContext dbContext, ILogger<ProcessPaymentRequestRepository> logger)
    : IProcessPaymentRequestRepository
{
    public async Task<PaymentRequest?> GetByIdAsync(Guid transactionId, CancellationToken ct)
    {
        return await dbContext.PaymentRequests.FindAsync([transactionId], ct);
    }

    public async Task UpdateStatusAsync(PaymentRequest entity, PaymentRequestStatus status, CancellationToken ct)
    {
        var previousStatus = entity.Status;
        entity.Status = status;
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Updated status for transaction {TransactionId}: {PreviousStatus} -> {NewStatus}",
            entity.TransactionId, previousStatus, status);
    }
}
