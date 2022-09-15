using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Common.Configuration;
using Common.Cryptography;
using Common.Logging;
using CustomerServices.Email;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class OrganizationEncryptionTests
    {
        private readonly IMapper _mapper;
        private DbContextOptions<CustomerContext> ContextOptions { get; }

        public OrganizationEncryptionTests()
        {
            ContextOptions = new DbContextOptionsBuilder<CustomerContext>()
                .UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
            new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(LocationDTO))); });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository, _mapper, Mock.Of<IEmailService>(),Mock.Of<IOptions<TechstepPartnerConfiguration>>());

            byte[] iv, key;
            var message = "Super secret data";
            var password = "123Password";
            key = Encryption.ComputeHashSha256(password);

            using (var aesAlg = Aes.Create())
            {
                iv = aesAlg.IV;
            }

            // Act
            var encryptedMessage = await customerService.EncryptDataForCustomer(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, message, key, iv);
            var decryptedMessage = await customerService.DecryptDataForCustomer(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, encryptedMessage, key, iv);

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