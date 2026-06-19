namespace PruebaPayment.Security;

public interface ICardDataProtector
{
    string Encrypt(string plaintext);
    string Decrypt(string encrypted);
}
