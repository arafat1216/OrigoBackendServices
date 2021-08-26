using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Cryptography
{
    public class Encryption
    {
        public static string EncryptData(string plainText, string salt, byte[] key, byte[] iv)
        {
            var dataToEncrypt = Encoding.UTF8.GetBytes(plainText + salt);
            var cipherText = Encrypt(dataToEncrypt, key, iv);

            return Convert.ToBase64String(cipherText);
        }

        public static string DecryptData(string cipherText, string salt, byte[] key, byte[] iv)
        {
            byte[] data = Convert.FromBase64String(cipherText);

            string decryptedData = Encoding.UTF8.GetString(Decrypt(data, key, iv));

            if (!decryptedData.EndsWith(salt))
                throw new CryptographicException("Decrypted message should end with salt");

            return decryptedData.Substring(0, decryptedData.Length - salt.Length);
        }

        /// <summary>
        /// Compute a sha256 hash value from given message.
        /// </summary>
        /// <param name="message">Value to generate hash from</param>
        /// <returns>new byte[32] hash value generated from given string</returns>
        public static byte[] ComputeHashSha256(string message)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            return hashedBytes;
        }

        /// <summary>
        /// Generate salt for testing purposes. In production we use the Customer Id as salt, but for quick testing and prototyping of code here, this is faster.
        /// </summary>
        /// <param name="length">Number of bytes to generate</param>
        /// <returns>A new byte[32] datatype filled with random values</returns>
        public static byte[] GenerateSalt(int length)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[length];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }

        /// <summary>
        /// Generate a cryptographic key from a passphrase and salt.
        /// </summary>
        /// <param name="toBeHashed">Value to be hashed, typically a string turned into a byte array</param>
        /// <param name="salt">Unique value to add noise to the resulting hash</param>
        /// <returns>A new byte[32] generated from given passphrase and salt value hashed over numberOfRounds times</returns>
        public static byte[] HashPassword(byte[] toBeHashed, byte[] salt)
        {
            int numberOfRounds = 10000; // If a value x is hashed twice with different number of rounds, their hash will differ

            using (var rfc2898 = new Rfc2898DeriveBytes(toBeHashed, salt, numberOfRounds, HashAlgorithmName.SHA256))
            {
                return rfc2898.GetBytes(32);
            }
        }


        private static byte[] Encrypt(byte[] dataToEncrypt, byte[] key, byte[] iv)
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = key;
                aes.IV = iv;

                using (var memoryStream = new MemoryStream())
                {
                    var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                    cryptoStream.FlushFinalBlock();

                    return memoryStream.ToArray();
                }
            }
        }

        private static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = key;
                aes.IV = iv;

                using (var memoryStream = new MemoryStream())
                {
                    var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(cipherText, 0, cipherText.Length);
                    cryptoStream.FlushFinalBlock();

                    var decryptBytes = memoryStream.ToArray();

                    return decryptBytes;
                }
            }
        }
    }
}
