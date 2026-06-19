using PruebaPayment.Features.Merchants.GetMerchants;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.Merchants;
public class MerchantsServicesRegistration : IServiceRegistration
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        // Search services
        builder.Services.AddScoped<IGetMerchantsService, GetMerchantsService>();
        builder.Services.AddScoped<IGetMerchantsRepository, GetMerchantsRepository>();
    }
}
