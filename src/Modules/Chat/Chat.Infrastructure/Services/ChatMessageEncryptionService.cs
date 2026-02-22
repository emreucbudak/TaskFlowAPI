using System.Security.Cryptography;
using System.Text;
using Chat.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Chat.Infrastructure.Services
{
    public class ChatMessageEncryptionService : IChatMessageEncryptionService
    {
        private const string PayloadPrefix = "tfmsgv1";
        private const int NonceSize = 12;
        private const int TagSize = 16;

        private readonly byte[] _encryptionKey;

        public ChatMessageEncryptionService(IConfiguration configuration)
        {
            var configuredKey = configuration["ChatEncryption:Key"]?.Trim();
            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                throw new InvalidOperationException("Mesaj sifreleme anahtari bulunamadi. ChatEncryption:Key zorunludur.");
            }

            try
            {
                var rawKey = Convert.FromBase64String(configuredKey);
                if (rawKey.Length != 32)
                {
                    throw new InvalidOperationException("ChatEncryption:Key base64 formatinda 32 byte (AES-256) olmalidir.");
                }

                _encryptionKey = rawKey;
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("ChatEncryption:Key gecersiz base64 formatinda.", ex);
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            if (IsEncrypted(plainText))
            {
                return plainText;
            }

            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = new byte[plaintextBytes.Length];
            var tag = new byte[TagSize];

            using var aesGcm = new AesGcm(_encryptionKey, TagSize);
            aesGcm.Encrypt(nonce, plaintextBytes, cipherBytes, tag);

            var nonceEncoded = Convert.ToBase64String(nonce);
            var tagEncoded = Convert.ToBase64String(tag);
            var cipherEncoded = Convert.ToBase64String(cipherBytes);

            return $"{PayloadPrefix}.{nonceEncoded}.{tagEncoded}.{cipherEncoded}";
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            if (!IsEncrypted(cipherText))
            {
                return cipherText;
            }

            var parts = cipherText.Split('.', 4, StringSplitOptions.None);
            if (parts.Length != 4 || !string.Equals(parts[0], PayloadPrefix, StringComparison.Ordinal))
            {
                throw new CryptographicException("Mesaj sifreleme paketi gecersiz.");
            }

            var nonce = Convert.FromBase64String(parts[1]);
            var tag = Convert.FromBase64String(parts[2]);
            var cipherBytes = Convert.FromBase64String(parts[3]);

            var plaintextBytes = new byte[cipherBytes.Length];
            using var aesGcm = new AesGcm(_encryptionKey, TagSize);
            aesGcm.Decrypt(nonce, cipherBytes, tag, plaintextBytes);

            return Encoding.UTF8.GetString(plaintextBytes);
        }

        public bool IsEncrypted(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.StartsWith($"{PayloadPrefix}.", StringComparison.Ordinal);
        }
    }
}
