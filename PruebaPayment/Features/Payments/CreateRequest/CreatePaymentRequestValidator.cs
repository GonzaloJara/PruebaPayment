using PruebaPayment.CommonModels;
using PruebaPayment.Utils;

namespace PruebaPayment.Features.Payments.CreateRequest;

public static class CreatePaymentRequestValidator
{
    private static readonly HashSet<string> _supportedCurrencies = ["CLP", "USD", "EUR"];
    public static bool Validate(CreatePaymentRequest request, out List<ValidationError> errors)
    {
        errors = [];

        if (request.IdempotencyKey == Guid.Empty)
        {
            errors.Add(new(nameof(request.IdempotencyKey), $"The field '{nameof(request.IdempotencyKey)}' is required and must be a valid non-empty GUID"));
        }

        if (request.MerchantId == Guid.Empty)
        {
            errors.Add(new(nameof(request.MerchantId), $"The field '{nameof(request.MerchantId)}' is required and must be a valid non-empty GUID"));
        }

        if (request.Amount <= 0)
        {
            errors.Add(new(nameof(request.Amount), $"The field '{nameof(request.Amount)}' must be greater than zero"));
        }
        else if (request.Amount > 1_000_000_000)
        {
            errors.Add(new(nameof(request.Amount), $"The field '{nameof(request.Amount)}' must be less than 1.000.000.000"));
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            errors.Add(new(nameof(request.Currency), $"The field '{nameof(request.Currency)}' is required"));
        }
        else if (!_supportedCurrencies.Contains(request.Currency.ToUpperInvariant()))
        {
            errors.Add(new(nameof(request.Currency), $"The field '{nameof(request.Currency)}' must be one of: {string.Join(", ", _supportedCurrencies)}"));
        }

        if (request.Card is null)
        {
            errors.Add(new(nameof(request.Card), $"The field '{nameof(request.Card)}' is required"));
        }
        else
        {
            ValidateCard(request.Card, errors);
        }

        return errors.Count == 0;
    }

    private static void ValidateCard(Card card, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(card.Numero))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Numero)}", $"'{nameof(card.Numero)}' is required"));
        }
        else if (!RegExUtils.CardNumberRegex().IsMatch(card.Numero.Replace(" ", "")))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Numero)}", $"'{nameof(card.Numero)}' must contain between 13 and 19 digits"));
        }

        if (string.IsNullOrWhiteSpace(card.Vencimiento))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Vencimiento)}", $"'{nameof(card.Vencimiento)}' is required"));
        }
        else if (!RegExUtils.ExpiryDateRegex().IsMatch(card.Vencimiento))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Vencimiento)}", $"'{nameof(card.Vencimiento)}' must be in the format 'MM/YY'"));
        }
        else if (IsCardExpired(card.Vencimiento))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Vencimiento)}", "Card is expired"));
        }

        if (string.IsNullOrWhiteSpace(card.Cvv))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Cvv)}", $"'{nameof(card.Cvv)}' is required"));
        }
        else if (!RegExUtils.CvvRegex().IsMatch(card.Cvv))
        {
            errors.Add(new($"{nameof(Card)}.{nameof(card.Cvv)}", $"'{nameof(card.Cvv)}' must contain between 3 and 4 digits"));
        }
    }

    private static bool IsCardExpired(string vencimiento)
    {
        var parts = vencimiento.Split('/');
        if (parts.Length != 2) return true;

        if (!int.TryParse(parts[0], out var month) || !int.TryParse(parts[1], out var year))
            return true;

        var expiry = new DateOnly(2000 + year, month, 1).AddMonths(1);
        return expiry <= DateOnly.FromDateTime(DateTime.UtcNow);
    }
}