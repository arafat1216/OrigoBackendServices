using System;
using System.Security.Cryptography;
using System.Text;
using Common.Cryptography;
using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class CustomerEncryptionTests : CustomerServicesBaseTest
    {
        public CustomerEncryptionTests() : base(
        new DbContextOptionsBuilder<CustomerContext>()
        // ReSharper disable once StringLiteralTypo
        .UseSqlite("Data Source=sqlitecustomerencryptionunittests.db")
            .Options
        )
        {
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            byte[] iv, key;
            var message = "Super secret data";
            var password = "123Password";
            key = Encryption.ComputeHashSha256(password);

            using (var aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            // Act
            var encryptedMessage = await customerService.EncryptDataForCustomer(CUSTOMER_ONE_ID, message, key, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer(CUSTOMER_ONE_ID, encryptedMessage, key, iv);

            // Assert
            Assert.Equal(message, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void TestEncryptDecryptOuter()
        {
            byte[] iv;
            var plaintext = "Super secret data";
            var password = "123Password";
            var salt = "salt";

            var key = Encryption.ComputeHashSha256(password);

            using (var aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            var encryptedMessage = Encryption.EncryptData(plaintext, salt, key, iv);
            var decryptedMessage = Encryption.DecryptData(encryptedMessage, salt, key, iv);
            Assert.Equal(plaintext, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void TestEncryptDecryptKeyDerivation()
        {
            byte[] iv, key, salt;
            var plaintext = "Super secret data";
            var password = "123Password";
            salt = Encryption.GenerateSalt(32);

            key = Encryption.HashPassword(Encoding.UTF8.GetBytes(password), salt);

            using (var aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            var encryptedMessage = Encryption.EncryptData(plaintext, Convert.ToBase64String(salt), key, iv);
            var decryptedMessage = Encryption.DecryptData(encryptedMessage, Convert.ToBase64String(salt), key, iv);
            Assert.Equal(plaintext, decryptedMessage);
        }
    }
}