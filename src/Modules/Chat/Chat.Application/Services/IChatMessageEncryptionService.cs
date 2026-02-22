namespace Chat.Application.Services
{
    public interface IChatMessageEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        bool IsEncrypted(string value);
    }
}
