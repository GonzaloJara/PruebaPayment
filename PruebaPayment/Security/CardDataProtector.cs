using Microsoft.Extensions.Options;
using PruebaPayment.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace PruebaPayment.Security;

// AES-256-GCM encryption so encrypted card data can later be decrypted (e.g. for refunds), unlike a one-way hash
public class CardDataProtector : ICardDataProtector
{
    private const int NonceSizeInBytes = 12;
    private const int TagSizeInBytes = 16;

    private readonly byte[] _key;

    public CardDataProtector(IOptions<CardEncryptionSettings> options)
    {
        _key = Convert.FromBase64String(options.Value.Key);
    }

    public string Encrypt(string plaintext)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeInBytes);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSizeInBytes];

        using var aesGcm = new AesGcm(_key, TagSizeInBytes);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var result = new byte[NonceSizeInBytes + ciphertext.Length + TagSizeInBytes];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSizeInBytes);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSizeInBytes, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSizeInBytes + ciphertext.Length, TagSizeInBytes);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encrypted)
    {
        var data = Convert.FromBase64String(encrypted);

        var nonce = data[..NonceSizeInBytes];
        var ciphertext = data[NonceSizeInBytes..^TagSizeInBytes];
        var tag = data[^TagSizeInBytes..];

        var plaintextBytes = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(_key, TagSizeInBytes);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
