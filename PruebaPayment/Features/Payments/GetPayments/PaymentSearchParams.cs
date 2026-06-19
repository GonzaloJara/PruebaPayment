using PruebaPayment.CommonModels;

namespace PruebaPayment.Features.Payments.GetPayments;

public class PaymentSearchParams() : PaginationAndOrderingParams
{
    public Guid? MerchantId { get; set; }
    public string? Status { get; set; }

    public new string ToCacheKey()
        => $"payments_{MerchantId}_{Status}_{base.ToCacheKey()}";
}
