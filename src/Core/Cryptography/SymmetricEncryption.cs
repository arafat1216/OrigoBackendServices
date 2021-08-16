﻿using System;
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
        public static byte[] Encrypt(string message, byte[] key, string salt, string pepper, byte[] iv)
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

        public static string Decrypt(byte[] cipherText, byte[] key, string salt, string pepper, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var memoryStream = new MemoryStream(cipherText);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            string decryptedMessage = streamReader.ReadToEnd();
            return decryptedMessage.Replace(salt, "").Replace(pepper, ""); // improvement phase: Seems like a bad way to do it. Check for best practice
        }

        public static byte[] ComputeHash(string message)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            return hashedBytes;
        }
    }
}
