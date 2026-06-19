using PruebaPayment.Features.Payments.CreateRequest;
using PruebaPayment.Features.Payments.GetPaymentById;
using PruebaPayment.Features.Payments.GetPayments;
using PruebaPayment.Features.TransactionEvents.GetTransactionEvents;
using PruebaPayment.Startup.DI;

namespace PruebaPayment.Features.Payments;

public class PaymentsEndpointsRegistration : IEndpointRegistration
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/payments")
            .WithTags("Payments");

        group.MapGet("", GetPaymentsEndpoint.GetAll);
        group.MapGet("orderbycolumns", GetPaymentsOrderByColumnsEndpoint.GetOrderByColumnsAll);

        group.MapGet("{id}", GetPaymentByIdEndpoint.GetById);
        group.MapGet("{id}/events", GetTransactionEventsEndpoint.GetByTransactionId);
        group.MapPost("", CreatePaymentRequestEndpoint.Create);
    }
}
