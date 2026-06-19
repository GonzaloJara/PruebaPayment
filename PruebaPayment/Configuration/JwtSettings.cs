namespace PruebaPayment.Configuration;

public class JwtSettings
{
    public const string Section = "Jwt";

    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessTokenExpiryMinutes { get; init; } = 15;
}
