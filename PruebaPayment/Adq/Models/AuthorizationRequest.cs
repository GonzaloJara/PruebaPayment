namespace PruebaPayment.Adq.Models;

// Campos inspirados en el mensaje ISO 8583 0100 (Authorization Request), simplificados a JSON
public record AuthorizationRequest(
    string Mti,
    string Pan,
    string ProcessingCode,
    decimal Amount,
    string TransactionCurrencyCode,
    string Stan,
    Guid MerchantId);
