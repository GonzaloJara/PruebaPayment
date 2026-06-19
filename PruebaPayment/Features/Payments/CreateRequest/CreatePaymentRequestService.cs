using Microsoft.Extensions.Caching.Hybrid;
using PruebaPayment.CommonModels;
using PruebaPayment.DB;
using PruebaPayment.DB.Models;
using PruebaPayment.Features.Payments.ViewModels;
using PruebaPayment.Features.TransactionEvents;
using PruebaPayment.Messaging;
using PruebaPayment.Messaging.Events;
using PruebaPayment.Security;
using PruebaPayment.Startup.Configure;

namespace PruebaPayment.Features.Payments.CreateRequest;

public interface ICreatePaymentRequestService
{
    Task<Result<PaymentViewModel>> CreateAsync(CreatePaymentRequest request, CancellationToken ct);
}

public class CreatePaymentRequestService(
    ICreatePaymentRequestRepository repository,
    HybridCache cache,
    ICardDataProtector cardDataProtector,
    IEventPublisher eventPublisher,
    ITransactionEventRecorder eventRecorder,
    ILogger<CreatePaymentRequestService> logger)
    : ICreatePaymentRequestService
{
    private const int DEFAULT_IDEMPOTENCY_CACHE_TIMEOUT_IN_SECONDS = 24 * 60 * 60; // 24 hours
    private readonly HybridCacheEntryOptions options = new()
    {
        Expiration = TimeSpan.FromSeconds(DEFAULT_IDEMPOTENCY_CACHE_TIMEOUT_IN_SECONDS)
    };

    public async Task<Result<PaymentViewModel>> CreateAsync(CreatePaymentRequest request, CancellationToken ct)
    {
        // Any subsequent request with the same idempotency key is served from the cache instead of being re-processed
        string cacheKey = $"idempotency_{request.IdempotencyKey}";

        PaymentViewModel? viewModel = await cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            if (!await repository.IsMerchantActiveAsync(request.MerchantId, ct))
            {
                logger.LogWarning("Merchant {MerchantId} is not active, rejecting payment request", request.MerchantId);
                return null;
            }

            // Map from request (Arguably, request gets here already validated)
            var entity = new PaymentRequest()
            {
                MerchantId = request.MerchantId,
                IdempotencyKey = request.IdempotencyKey,
                Status = PaymentRequestStatus.Pending,
                Amount = request.Amount,
                Currency = request.Currency,
                EncryptedCardData = cardDataProtector.Encrypt($"{request.Card.Numero}|{request.Card.Vencimiento}|{request.Card.Cvv}")
            };

            // Create on DB
            await repository.CreateAsync(entity, ct);

            logger.LogInformation(
                "Created payment request, transaction {TransactionId} for merchant {MerchantId}, amount {Amount} {Currency}",
                entity.TransactionId, entity.MerchantId, entity.Amount, entity.Currency);

            await eventRecorder.RecordAsync(
                entity.TransactionId,
                TransactionEventType.Created,
                TransactionEventLevel.Info,
                $"Payment request created for merchant {entity.MerchantId}, amount {entity.Amount} {entity.Currency}",
                ct: ct);

            // Evict cache (so new queries gets the new data)
            await cache.RemoveByTagAsync("payments", ct);

            // Publish message to the queue for async processing
            // This is technical debt as it should be implementing an outbox pattern
            await eventPublisher.PublishAsync(
                RabbitMqStartup.PaymentEventsExchange,
                RabbitMqStartup.PaymentCreatedRoutingKey,
                PaymentCreatedEvent.From(entity),
                ct);

            logger.LogInformation("Enqueued PaymentCreatedEvent for transaction {TransactionId}", entity.TransactionId);

            return PaymentViewModel.From(entity);
        },
        options: options,
        cancellationToken: ct);

        if(viewModel is null)
        {
            await cache.RemoveAsync(cacheKey, ct);
            return Result<PaymentViewModel>.Failure(CreatePaymentRequestErrors.MerchantNotActive(request.MerchantId));
        }

        return Result<PaymentViewModel>.Success(viewModel);
    }
}