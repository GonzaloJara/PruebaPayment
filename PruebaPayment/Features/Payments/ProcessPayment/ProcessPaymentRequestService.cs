using Microsoft.Extensions.Caching.Hybrid;
using PruebaPayment.Adq;
using PruebaPayment.DB.Models;
using PruebaPayment.Features.TransactionEvents;
using PruebaPayment.Security;

namespace PruebaPayment.Features.Payments.ProcessPayment;

public interface IProcessPaymentRequestService
{
    Task ProcessAsync(Guid transactionId, CancellationToken ct);
}

public class ProcessPaymentRequestService(
    IProcessPaymentRequestRepository repository,
    ICardDataProtector cardDataProtector,
    IAdqClient adqClient,
    HybridCache cache,
    ITransactionEventRecorder eventRecorder,
    ILogger<ProcessPaymentRequestService> logger)
    : IProcessPaymentRequestService
{
    public async Task ProcessAsync(Guid transactionId, CancellationToken ct)
    {
        logger.LogInformation("Processing payment request for transaction {TransactionId}", transactionId);

        var entity = await repository.GetByIdAsync(transactionId, ct);
        if (entity is null)
        {
            logger.LogWarning("PaymentRequest {TransactionId} not found, skipping processing", transactionId);
            return;
        }

        await repository.UpdateStatusAsync(entity, PaymentRequestStatus.Processsing, ct);
        await cache.RemoveAsync($"payment_{transactionId}", ct);

        await eventRecorder.RecordAsync(
            transactionId,
            TransactionEventType.Processing,
            TransactionEventLevel.Info,
            "Started processing payment request",
            ct: ct);

        var pan = cardDataProtector.Decrypt(entity.EncryptedCardData).Split('|')[0];

        logger.LogInformation("Calling Adq authorize for transaction {TransactionId}", transactionId);
        var finalStatus = await adqClient.AuthorizeAsync(entity, pan, ct);
        logger.LogInformation("Adq authorize returned {Status} for transaction {TransactionId}", finalStatus, transactionId);

        await repository.UpdateStatusAsync(entity, finalStatus, ct);
        await cache.RemoveAsync($"payment_{transactionId}", ct);
        await cache.RemoveByTagAsync("payments", ct);

        await eventRecorder.RecordAsync(
            transactionId,
            TransactionEventType.Result,
            LevelForStatus(finalStatus),
            $"Transaction finished with status {finalStatus}",
            ct: ct);

        logger.LogInformation("Finished processing transaction {TransactionId} with final status {Status}", transactionId, finalStatus);
    }

    private static TransactionEventLevel LevelForStatus(PaymentRequestStatus status) => status switch
    {
        PaymentRequestStatus.Approved => TransactionEventLevel.Info,
        PaymentRequestStatus.Declined => TransactionEventLevel.Warning,
        PaymentRequestStatus.Failed => TransactionEventLevel.Error,
        _ => TransactionEventLevel.Info
    };
}
