using PruebaPayment.CommonModels;

namespace PruebaPayment.Features.Payments.GetPaymentById;

public static class GetPaymentRequestByIdErrors
{
    public static Error PaymentNotFoundWithId(Guid id)
        => new("PAY_GID_01", $"No payment with id='{id}' was found");

}