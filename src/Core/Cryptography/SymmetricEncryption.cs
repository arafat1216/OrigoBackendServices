using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Common.Cryptography
{
    /// <summary>
    /// [Prototype] Encryption/decryption utility class
    /// 
    /// Allows encryption and decryption of strings using AES (Advanced Encryption Standard)
    /// 
    /// Improvement phase: Add more encryption algorithms? Allow more configuration options?
    /// </summary>
    public static class SymmetricEncryption
    {
        public static byte[] Encrypt(string message, byte[] key, string salt, string pepper, byte[] iv)
        {
            try
            {
                using var aesAlg = Aes.Create();

                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using var memoryStream = new MemoryStream();
                using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(message + salt + pepper);
                }
                return memoryStream.ToArray();
            }
            catch(Exception)
            {
                // improvement phase: Log error
                return Array.Empty<byte>();
            }
        }

        public static string Decrypt(byte[] cipherText, byte[] key, string salt, string pepper, byte[] iv)
        {
            try
            {
                using var aesAlg = Aes.Create();
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using var memoryStream = new MemoryStream(cipherText);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using var streamReader = new StreamReader(cryptoStream);
                string decryptedMessage = streamReader.ReadToEnd();
                if (!decryptedMessage.EndsWith(salt + pepper))
                    throw new CryptographicException("Decrypted message should end with salt and pepper");

                return decryptedMessage.Substring(0, decryptedMessage.Length - salt.Length - pepper.Length);
            }
            catch (CryptographicException ex)
            {
                // Improvement phase: log error
                return null;
            }
            catch (Exception)
            {
                return null;
            }
           
        }

        public static byte[] ComputeHash(string message)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            return hashedBytes;
        }
    }
}
