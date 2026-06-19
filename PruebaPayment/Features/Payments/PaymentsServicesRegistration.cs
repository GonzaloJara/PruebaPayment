using PruebaPayment.Adq;
using PruebaPayment.Configuration;
using PruebaPayment.Features.Payments.CreateRequest;
using PruebaPayment.Features.Payments.GetPaymentById;
using PruebaPayment.Features.Payments.GetPayments;
using PruebaPayment.Features.Payments.ProcessPayment;
using PruebaPayment.Security;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.Payments;
public class PaymentsServicesRegistration : IServiceRegistration
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        // Get by id services
        builder.Services.AddScoped<IGetPaymentRequestByIdService, GetPaymentRequestByIdService>();
        builder.Services.AddScoped<IGetPaymentRequestByIdRepository, GetPaymentRequestByIdRepository>();

        // Search services
        builder.Services.AddScoped<IGetPaymentsService, GetPaymentsService>();
        builder.Services.AddScoped<IGetPaymentsRepository, GetPaymentsRepository>();

        // Create services
        builder.Services.AddScoped<ICreatePaymentRequestService, CreatePaymentRequestService>();
        builder.Services.AddScoped<ICreatePaymentRequestRepository, CreatePaymentRequestRepository>();

        // Process services (consumer of PaymentCreatedEvent)
        builder.Services.AddScoped<IProcessPaymentRequestService, ProcessPaymentRequestService>();
        builder.Services.AddScoped<IProcessPaymentRequestRepository, ProcessPaymentRequestRepository>();
        builder.Services.AddScoped<IAdqClient, AdqClient>();
        builder.Services.AddHostedService<PaymentRequestCreatedConsumer>();

        // Card data encryption
        builder.Services.Configure<CardEncryptionSettings>(builder.Configuration.GetSection(CardEncryptionSettings.Section));
        builder.Services.AddSingleton<ICardDataProtector, CardDataProtector>();
    }
}
