using Microsoft.Extensions.Caching.Hybrid;
using PruebaPayment.CommonModels;
using PruebaPayment.DB.Models;
using PruebaPayment.Features.Payments.ViewModels;
using PruebaPayment.Features.TransactionEvents;

namespace PruebaPayment.Features.Payments.GetPaymentById;

public interface IGetPaymentRequestByIdService
{
    Task<Result<PaymentViewModel>> GetByIdAsync(Guid id, CancellationToken ct);
}

public class GetPaymentRequestByIdService(
    IGetPaymentRequestByIdRepository repository,
    HybridCache cache,
    ITransactionEventRecorder eventRecorder)
    : IGetPaymentRequestByIdService
{
    private const int DEFAULT_PAYMENT_GET_BY_ID_CACHE_TIMEOUT_IN_SECONDS = 5 * 60; // 5 Minutes
    private readonly HybridCacheEntryOptions options = new()
    {
        Expiration = TimeSpan.FromSeconds(DEFAULT_PAYMENT_GET_BY_ID_CACHE_TIMEOUT_IN_SECONDS)
    };

    public async Task<Result<PaymentViewModel>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        // Cache key
        string cacheKey = $"payment_{id}";

        // Ejecutar sql u obtener del caché si existe la cacheKey
        PaymentRequest? entity = await cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            return await repository.GetByIdAsync(id, ct);
        },
        options: options,
        cancellationToken: ct);

        // Not found
        if (entity is null)
        {
            await eventRecorder.RecordAsync(
                id,
                TransactionEventType.InfoRequested,
                TransactionEventLevel.Warning,
                "Payment info requested for an unknown transaction id",
                ct: ct);

            return Result<PaymentViewModel>.Failure(GetPaymentRequestByIdErrors.PaymentNotFoundWithId(id));
        }

        await eventRecorder.RecordAsync(
            id,
            TransactionEventType.InfoRequested,
            TransactionEventLevel.Info,
            "Payment info requested",
            ct: ct);

        // Found
        return Result<PaymentViewModel>.Success(PaymentViewModel.From(entity));
    }
}