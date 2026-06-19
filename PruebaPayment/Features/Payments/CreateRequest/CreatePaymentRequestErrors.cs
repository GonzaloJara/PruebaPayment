using PruebaPayment.CommonModels;

namespace PruebaPayment.Features.Payments.CreateRequest;

public static class CreatePaymentRequestErrors
{
    public static Error ValidationError()
        => new($"PAY_CREATE_00", $"An error occured while attempting to validate request data");

    public static Error MerchantNotActive(Guid merchantId)
        => new($"PAY_CREATE_01", $"Merchant '{merchantId}' is not active or does not exist");
}