using PruebaPayment.CommonModels;
using PruebaPayment.Features.Merchants.ViewModels;

namespace PruebaPayment.Features.Merchants.GetMerchants;

public static class GetMerchantsEndpoint
{
    public static async Task<IResult> GetAll(
        [AsParameters] MerchantSearchParams searchParams,
        IGetMerchantsService service,
        CancellationToken ct)
    {
        Result<PagedResult<MerchantViewModel>> result = await service.GetAllAsync(searchParams, ct);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
