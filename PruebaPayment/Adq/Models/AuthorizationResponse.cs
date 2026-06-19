namespace PruebaPayment.Adq.Models;

// Campos inspirados en el mensaje ISO 8583 0110 (Authorization Response), simplificados a JSON
public record AuthorizationResponse(
    string Mti,
    string Stan,
    string ResponseCode,
    string ResponseMessage,
    string? AuthorizationCode);
