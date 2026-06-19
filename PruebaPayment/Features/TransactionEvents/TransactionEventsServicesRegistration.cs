using PruebaPayment.Features.TransactionEvents.GetTransactionEvents;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.TransactionEvents;

public class TransactionEventsServicesRegistration : IServiceRegistration
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITransactionEventRecorder, TransactionEventRecorder>();

        builder.Services.AddScoped<IGetTransactionEventsService, GetTransactionEventsService>();
        builder.Services.AddScoped<IGetTransactionEventsRepository, GetTransactionEventsRepository>();
    }
}
