namespace PruebaPayment.Features.Merchants.GetMerchants;

public static class GetMerchantsOrderByColumnsEndpoint
{
    public static IResult GetOrderByColumnsAll(IGetMerchantsService service)
    {
        var columns = service.GetOrderByColumns();
        return Results.Ok(new { Columns = columns });
    }
}
