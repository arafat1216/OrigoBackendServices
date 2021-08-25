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
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
        public async void EncryptDecryptMessage_Valid()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            byte[] pepper, iv, key;
            string message = "Super secret data";
            string password = "123Password";
            key = SymmetricEncryption.ComputeHash(password);

            using (Aes aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            pepper = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); // fill pepper with strong random bytes
            rng.GetBytes(pepper);

            // Act
            var encryptedMessage = await customerService.EncryptDataForCustomer(CUSTOMER_ONE_ID, message, key, pepper, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer(CUSTOMER_ONE_ID, encryptedMessage, key, pepper, iv);

            // Assert
            Assert.Equal(message, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage_Invalid()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            byte[] pepper, iv, keyValid, keyInvalid;
            string message = "Super secret data";
            string encryptPassword = "123Password";
            string decryptPassword = "faultypassword";
            keyValid = SymmetricEncryption.ComputeHash(encryptPassword);
            keyInvalid = SymmetricEncryption.ComputeHash(decryptPassword);

            using (Aes aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            pepper = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); // fill pepper with strong random bytes
            rng.GetBytes(pepper);

            // Act
            var encryptedMessage = await customerService.EncryptDataForCustomer(CUSTOMER_ONE_ID, message, keyValid, pepper, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer(CUSTOMER_ONE_ID, encryptedMessage, keyInvalid, pepper, iv);

            // Assert
            Assert.NotEqual(message, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage2_Valid()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
            var encryptedMessage = await customerService.EncryptDataForCustomer2(CUSTOMER_ONE_ID, message, key, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer2(CUSTOMER_ONE_ID, encryptedMessage, key, iv);

            // Assert
            Assert.Equal(message, decryptedMessage);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void TestEncryptDecryptInner()
        {
            byte[] iv, key;
            string plaintext = "Super secret data";
            string password = "123Password";

            key = Encryption.ComputeHashSha256(password);

            using (Aes aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            var encryptedMessage = Encryption.Encrypt(Encoding.UTF8.GetBytes(plaintext), key, iv);
            var decryptedMessage = Encryption.Decrypt(encryptedMessage, key, iv);
            string cipherDecrypted = Encoding.UTF8.GetString(decryptedMessage);
            Assert.Equal(plaintext, cipherDecrypted);
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
    }
}
