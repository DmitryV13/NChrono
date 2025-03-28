using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Helpers
{
    public static class CookieGenerator
    {
        // Salt used for key derivation
        private const string SaltData = "QADLz4qk3rVgBSGjDfAH3XWVqKKagMXezBPv7TmXvwnXDDeRpHaLBv4JnTGRwLg9tzbmV77g8DUEAEa6JPv66hy7SwHBL4z4FbGdh2MVs4kq9RcaZEAszuP5ccLsEfqCpwdSvVVt479DCZrwjSHrJVwaja9WQaWAmEY9NsPvEHKnFwHTGAvPXpjpCxkbedYquEauLvZLphwmJLUteZ4QAXU6Z4F3PDmh3wsQXvSctQBHvNWf";
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes(SaltData);

        // Method to create an encrypted cookie value
        public static string Create(string value)
        {
            // Encrypt the given value using AES and return the encrypted string
            return EncryptStringAes(value, "BjXNmq5MKKaraLwxz9uaATvFwE4Rj679KguTRE8c2j56FnkuKJKfkGbZEeDGFDvsGYNHpUXFUUUuUHBR4UV3T2kumguhubg6Gpt7CyqGDbUPrMvPc67kX3yP");
        }

        // Method to validate and decrypt a cookie value
        public static string Validate(string value)
        {
            // Decrypt the given encrypted value using AES and return the plaintext
            return DecryptStringAes(value, "BjXNmq5MKKaraLwxz9uaATvFwE4Rj679KguTRE8c2j56FnkuKJKfkGbZEeDGFDvsGYNHpUXFUUUuUHBR4UV3T2kumguhubg6Gpt7CyqGDbUPrMvPc67kX3yP");
        }

        // Method to encrypt a plaintext string using AES encryption
        private static string EncryptStringAes(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException(nameof(sharedSecret));

            string outStr;
            Aes aesAlg = null;

            try
            {
                // Generate a key from the shared secret using PBKDF2
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                aesAlg = Aes.Create();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8); // Set the AES key

                // Create an encryptor to perform the encryption
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    // Write the IV length and IV to the stream
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    // Use CryptoStream to encrypt the plaintext
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    // Convert the encrypted data to a Base64 string
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                aesAlg?.Dispose(); // Dispose of the AES algorithm
            }

            return outStr;
        }

        // Method to decrypt an AES-encrypted string
        private static string DecryptStringAes(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException(nameof(sharedSecret));

            Aes aesAlg = null;
            string plaintext;

            try
            {
                // Generate a key from the shared secret using PBKDF2
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                var bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = Aes.Create();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8); // Set the AES key
                    aesAlg.IV = ReadByteArray(msDecrypt); // Read the IV from the stream

                    // Create a decryptor to perform the decryption
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd(); // Read the decrypted data
                        }
                    }
                }
            }
            finally
            {
                aesAlg?.Dispose(); // Dispose of the AES algorithm
            }

            return plaintext;
        }

        // Method to read a byte array from a stream
        private static byte[] ReadByteArray(Stream s)
        {
            // Read the length of the byte array
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            // Read the byte array itself
            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
