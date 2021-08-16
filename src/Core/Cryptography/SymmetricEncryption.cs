using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static byte[] Encrypt(string message, string salt, string passwordAndSalt, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            
            aesAlg.Key = ComputeHash(passwordAndSalt);
            aesAlg.IV = iv;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(message); // Write all data to the stream.
            }
            return ms.ToArray();
        }

        public static string Decrypt(byte[] cipherText, string passwordAndSalt, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = ComputeHash(passwordAndSalt);
            aesAlg.IV = iv;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public static byte[] ComputeHash(string message)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            return hashedBytes;//BitConverter.ToString(hashedBytes);
        }
    }
}
