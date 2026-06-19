using PruebaPayment.Features.Merchants.GetMerchants;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.Merchants;

public class MerchantsEndpointsRegistration : IEndpointRegistration
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/merchants")
            .WithTags("Merchants");

        group.MapGet("", GetMerchantsEndpoint.GetAll);
        group.MapGet("orderbycolumns", GetMerchantsOrderByColumnsEndpoint.GetOrderByColumnsAll);
    }
}
