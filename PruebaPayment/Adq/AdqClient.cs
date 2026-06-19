using System.Net.Http.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;
using PruebaPayment.Adq.Models;
using PruebaPayment.DB.Models;
using PruebaPayment.Features.TransactionEvents;

namespace PruebaPayment.Adq;

public interface IAdqClient
{
    Task<PaymentRequestStatus> AuthorizeAsync(PaymentRequest paymentRequest, string pan, CancellationToken ct);
}

public class AdqClient(
    IHttpClientFactory httpClientFactory,
    ITransactionEventRecorder eventRecorder,
    ILogger<AdqClient> logger)
    : IAdqClient
{
    public async Task<PaymentRequestStatus> AuthorizeAsync(PaymentRequest paymentRequest, string pan, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient("adq");

        var request = new AuthorizationRequest(
            Mti: "0100",
            Pan: pan,
            ProcessingCode: "00",
            Amount: paymentRequest.Amount,
            TransactionCurrencyCode: paymentRequest.Currency,
            Stan: paymentRequest.TransactionId.ToString("N")[..6],
            MerchantId: paymentRequest.MerchantId);

        logger.LogInformation("Calling Adq API to authorize transaction {TransactionId}", paymentRequest.TransactionId);

        await eventRecorder.RecordAsync(
            paymentRequest.TransactionId,
            TransactionEventType.AdqRequest,
            TransactionEventLevel.Info,
            "Sent authorization request to Adq",
            details: $"Stan={request.Stan}, Amount={request.Amount} {request.TransactionCurrencyCode}",
            ct: ct);

        try
        {
            var httpResponse = await client.PostAsJsonAsync("/authorize", request, ct);

            if (!httpResponse.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Adq authorization returned status {StatusCode} for transaction {TransactionId}",
                    httpResponse.StatusCode, paymentRequest.TransactionId);

                await eventRecorder.RecordAsync(
                    paymentRequest.TransactionId,
                    TransactionEventType.AdqResult,
                    TransactionEventLevel.Error,
                    $"Adq authorization call returned HTTP {httpResponse.StatusCode}",
                    ct: ct);

                return PaymentRequestStatus.Failed;
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<AuthorizationResponse>(ct);

            var status = response?.ResponseCode switch
            {
                "00" => PaymentRequestStatus.Approved,
                "05" => PaymentRequestStatus.Declined,
                _ => PaymentRequestStatus.Failed
            };

            logger.LogInformation(
                "Adq authorization response code {ResponseCode} for transaction {TransactionId}, mapped to {Status}",
                response?.ResponseCode, paymentRequest.TransactionId, status);

            await eventRecorder.RecordAsync(
                paymentRequest.TransactionId,
                TransactionEventType.AdqResult,
                LevelForStatus(status),
                $"Adq responded with code {response?.ResponseCode} ({response?.ResponseMessage}), mapped to {status}",
                ct: ct);

            return status;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or BrokenCircuitException or TimeoutRejectedException)
        {
            logger.LogError(ex, "Adq authorization request failed for transaction {TransactionId}", paymentRequest.TransactionId);

            await eventRecorder.RecordAsync(
                paymentRequest.TransactionId,
                TransactionEventType.AdqResult,
                TransactionEventLevel.Error,
                "Adq authorization request failed",
                details: ex.Message,
                ct: ct);

            return PaymentRequestStatus.Failed;
        }
    }

    private static TransactionEventLevel LevelForStatus(PaymentRequestStatus status) => status switch
    {
        PaymentRequestStatus.Approved => TransactionEventLevel.Info,
        PaymentRequestStatus.Declined => TransactionEventLevel.Warning,
        PaymentRequestStatus.Failed => TransactionEventLevel.Error,
        _ => TransactionEventLevel.Info
    };
}
