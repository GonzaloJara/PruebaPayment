namespace PruebaPayment.Configuration;

public class CardEncryptionSettings
{
    public const string Section = "CardEncryption";

    // Base64-encoded 256-bit (32 byte) AES key
    public required string Key { get; init; }
}
