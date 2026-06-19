using PruebaPayment.Features.Web.Form;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.Web;

public class WebEndpointRegistration : IEndpointRegistration
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("web")
            .WithTags("Web");

        group.MapGet("form", PaymentRequestFormWebEndpoint.PaymentRequestForm)
            .AllowAnonymous();
    }
}

