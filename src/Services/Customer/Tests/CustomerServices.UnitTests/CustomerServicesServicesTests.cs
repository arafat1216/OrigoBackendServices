using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Security.Cryptography;
using Common.Cryptography;
using System.Text;
using System;

namespace CustomerServices.UnitTests
{
    public class CustomerServicesServicesTests : CustomerServicesBaseTest
    {
        public CustomerServicesServicesTests()
            : base(
                new DbContextOptionsBuilder<CustomerContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomerunittests.db")
                    .Options
            )
        {

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetCompanyOne_CheckName()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            // Act
            var customer = await customerService.GetCustomerAsync(CUSTOMER_ONE_ID);

            // Assert
            Assert.Equal("COMPANY ONE", customer.CompanyName);
            Assert.Equal("My Way 1", customer.CompanyAddress.Street);
            Assert.Equal("1111", customer.CompanyAddress.PostCode);
            Assert.Equal("My City", customer.CompanyAddress.City);
            Assert.Equal("NO", customer.CompanyAddress.Country);
        } 

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            byte[] iv, key;
            string message = "Super secret data";
            string password = "123Password";
            key = Encryption.ComputeHashSha256(password);

            using (Aes aesAlg = Aes.Create())
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
        public async void TestEncryptDecryptOuter()
        {
            byte[] iv, key;
            string plaintext = "Super secret data";
            string password = "123Password";
            string salt = "salt";

            key = Encryption.ComputeHashSha256(password);

            using (Aes aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            var encryptedMessage = Encryption.EncryptData(plaintext, salt, key, iv);
            var decryptedMessage = Encryption.DecryptData(encryptedMessage, salt, key, iv);
            Assert.Equal(plaintext, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void TestEncryptDecryptKeyDerivation()
        {
            byte[] iv, key, salt;
            string plaintext = "Super secret data";
            string password = "123Password";
            salt = Encryption.GenerateSalt(32);

            key = Encryption.HashPassword(Encoding.UTF8.GetBytes(password), salt);

            using (Aes aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            var encryptedMessage = Encryption.EncryptData(plaintext, Convert.ToBase64String(salt), key, iv);
            var decryptedMessage = Encryption.DecryptData(encryptedMessage, Convert.ToBase64String(salt), key, iv);
            Assert.Equal(plaintext, decryptedMessage);
        }
    }
}
