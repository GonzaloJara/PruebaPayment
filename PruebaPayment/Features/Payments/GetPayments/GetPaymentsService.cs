using Microsoft.Extensions.Caching.Hybrid;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Payments.ViewModels;

namespace PruebaPayment.Features.Payments.GetPayments;

public interface IGetPaymentsService
{
    Task<Result<PagedResult<PaymentViewModel>>> GetAllAsync(PaymentSearchParams searchParams, CancellationToken ct);
    string[] GetOrderByColumns();
}


public class GetPaymentsService(
    IGetPaymentsRepository repository,
    HybridCache cache)
    : IGetPaymentsService
{
    private static readonly string[] _orderColumns = ["MerchantId", "CreatedAt"];
    private const int DEFAULT_PAYMENT_SEARCH_CACHE_TIMEOUT_IN_SECONDS = 5 * 60;
    private readonly HybridCacheEntryOptions options = new()
    {
        Expiration = TimeSpan.FromSeconds(DEFAULT_PAYMENT_SEARCH_CACHE_TIMEOUT_IN_SECONDS)
    };

    public string[] GetOrderByColumns() => _orderColumns;

    public async Task<Result<PagedResult<PaymentViewModel>>> GetAllAsync(PaymentSearchParams searchParams, CancellationToken ct)
    {
        string cacheKey = searchParams.ToCacheKey();

        var pagedResult = await cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            return await repository.GetAllAsync(searchParams, ct);
        },
        options: options,
        tags: ["payments"],
        cancellationToken: ct);

        return Result<PagedResult<PaymentViewModel>>.Success(pagedResult);
    }
}