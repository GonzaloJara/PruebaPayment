using PruebaPayment.CommonModels;

namespace PruebaPayment.Features.Merchants.GetMerchants;

public class MerchantSearchParams() : PaginationAndOrderingParams
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }

    public new string ToCacheKey()
        => $"merchants_{Name}_{IsActive}_{base.ToCacheKey()}";
}
