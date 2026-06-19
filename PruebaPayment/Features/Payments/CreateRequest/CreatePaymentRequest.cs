namespace PruebaPayment.Features.Payments.CreateRequest;

public record CreatePaymentRequest(Guid IdempotencyKey, Guid MerchantId, decimal Amount, string Currency, Card Card);

public record Card(string Numero, string Vencimiento, string Cvv);
