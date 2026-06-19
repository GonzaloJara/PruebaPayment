using PruebaPayment.CommonModels;
using PruebaPayment.Features.Payments.ViewModels;

namespace PruebaPayment.Features.Payments.GetPayments;

public static class GetPaymentsEndpoint
{
    public static async Task<IResult> GetAll(
        [AsParameters] PaymentSearchParams searchParams,
        IGetPaymentsService service,
        CancellationToken ct)
    {
        Result<PagedResult<PaymentViewModel>> result = await service.GetAllAsync(searchParams, ct);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
