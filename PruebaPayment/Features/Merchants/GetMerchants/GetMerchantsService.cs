using Microsoft.Extensions.Caching.Hybrid;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Merchants.ViewModels;

namespace PruebaPayment.Features.Merchants.GetMerchants;

public interface IGetMerchantsService
{
    Task<Result<PagedResult<MerchantViewModel>>> GetAllAsync(MerchantSearchParams searchParams, CancellationToken ct);
    string[] GetOrderByColumns();
}


public class GetMerchantsService(
    IGetMerchantsRepository repository,
    HybridCache cache)
    : IGetMerchantsService
{
    private static readonly string[] _orderColumns = ["Name", "IsActive"];
    private const int DEFAULT_MERCHANT_SEARCH_CACHE_TIMEOUT_IN_SECONDS = 5 * 60;
    private readonly HybridCacheEntryOptions options = new()
    {
        Expiration = TimeSpan.FromSeconds(DEFAULT_MERCHANT_SEARCH_CACHE_TIMEOUT_IN_SECONDS)
    };

    public string[] GetOrderByColumns() => _orderColumns;

    public async Task<Result<PagedResult<MerchantViewModel>>> GetAllAsync(MerchantSearchParams searchParams, CancellationToken ct)
    {
        string cacheKey = searchParams.ToCacheKey();

        var pagedResult = await cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            return await repository.GetAllAsync(searchParams, ct);
        },
        options: options,
        tags: ["merchants"],
        cancellationToken: ct);

        return Result<PagedResult<MerchantViewModel>>.Success(pagedResult);
    }
}
