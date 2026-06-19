namespace PruebaPayment.Features.Payments.GetPayments;

public static class GetPaymentsOrderByColumnsEndpoint
{
    public static IResult GetOrderByColumnsAll(IGetPaymentsService service)
    {
        var columns = service.GetOrderByColumns();
        return Results.Ok(new { Columns = columns });
    }
}