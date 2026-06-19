using Microsoft.AspNetCore.Mvc;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Payments.ViewModels;

namespace PruebaPayment.Features.Payments.CreateRequest;

public static class CreatePaymentRequestEndpoint
{
    public static async Task<IResult> Create(
        [FromBody] CreatePaymentRequest request,
        ICreatePaymentRequestService service,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("PruebaPayment.Features.Payments.CreateRequest.CreatePaymentRequestEndpoint");

        logger.LogInformation("Received create payment request for merchant {MerchantId}, idempotency key {IdempotencyKey}", request.MerchantId, request.IdempotencyKey);

        if (!CreatePaymentRequestValidator.Validate(request, out List<ValidationError> errors))
        {
            logger.LogWarning("Create payment request failed validation for merchant {MerchantId}", request.MerchantId);
            return Results.BadRequest(Result.ValidationFailure(CreatePaymentRequestErrors.ValidationError(), errors));
        }

        Result<PaymentViewModel> result = await service.CreateAsync(request, ct);

        if(!result.IsSuccess)
        {
            logger.LogWarning("Create payment request failed for merchant {MerchantId}: {ErrorCode}", request.MerchantId, result.Error?.Code);
            Results.BadRequest(result);
        }

        logger.LogInformation("Create payment request completed, transaction {TransactionId} returned to caller", result.Content?.TransactionId);

        return Results.Created($"payments/{result.Content?.TransactionId}", result.Content);
    }
}
