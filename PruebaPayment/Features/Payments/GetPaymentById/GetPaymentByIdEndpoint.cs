using Microsoft.AspNetCore.Mvc;
using PruebaPayment.CommonModels;
using PruebaPayment.Features.Payments.ViewModels;

namespace PruebaPayment.Features.Payments.GetPaymentById;

public static class GetPaymentByIdEndpoint
{
    public static async Task<IResult> GetById(
        [FromRoute] Guid id,
        IGetPaymentRequestByIdService service,
        CancellationToken ct)
    {
        Result<PaymentViewModel> result = await service.GetByIdAsync(id, ct);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound(result);
    }
}