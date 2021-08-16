using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Security.Cryptography;
using Common.Cryptography;

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
        public async void EncryptDecrypt_message()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);
            var customer = await customerService.GetCustomerAsync(CUSTOMER_ONE_ID);
            byte[] key, iv;
            string message = "Super secret data";
            string password = "123Password";
            using (Aes aesAlg = Aes.Create())
            {
                //key = aesAlg.Key;
                iv = aesAlg.IV;
            }
            //key = SymmetricEncryption.ComputeHash(password);


            // Act
            var encryptedMessage = await customerService.EncryptDataForCustomer(CUSTOMER_ONE_ID, message, password, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer(CUSTOMER_ONE_ID, encryptedMessage, password, iv);

            // Assert
            Assert.Equal(message, decryptedMessage);
        }
    }
}
