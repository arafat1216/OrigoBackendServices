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
using System.Linq;

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
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository);

            // Act
            var customer = await customerService.GetOrganizationAsync(CUSTOMER_ONE_ID, true, true);

            // Assert
            Assert.Equal("COMPANY ONE", customer.OrganizationName);
            Assert.Equal("My Way 1", customer.OrganizationAddress.Street);
            Assert.Equal("1111", customer.OrganizationAddress.PostCode);
            Assert.Equal("My City", customer.OrganizationAddress.City);
            Assert.Equal("NO", customer.OrganizationAddress.Country);

            // - Location
            Assert.Equal("COMPANY ONE", customer.OrganizationLocation.Name);
            Assert.Equal("Location of COMPANY ONE", customer.OrganizationLocation.Description);
            Assert.Equal("My Way 1A", customer.OrganizationLocation.Address1);
            Assert.Equal("My Way 1B", customer.OrganizationLocation.Address2);
            Assert.Equal("0585", customer.OrganizationLocation.PostalCode);
            Assert.Equal("Oslo", customer.OrganizationLocation.City);
            Assert.Equal("Norway", customer.OrganizationLocation.Country);

            // - preferences
            Assert.Equal("webPage 1", customer.OrganizationPreferences.WebPage);
            Assert.Equal("logoUrl 1", customer.OrganizationPreferences.LogoUrl);
            Assert.Equal("organizationNotes 1", customer.OrganizationPreferences.OrganizationNotes);
            Assert.True(customer.OrganizationPreferences.EnforceTwoFactorAuth);
            Assert.Equal("NO", customer.OrganizationPreferences.PrimaryLanguage);
            Assert.Equal(0, customer.OrganizationPreferences.DefaultDepartmentClassification);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllOrganizations()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository);

            // Act
            var organizations = await customerService.GetOrganizationsAsync(false);

            // Assert
            Assert.Equal("COMPANY ONE", organizations[0].OrganizationName);
            Assert.Equal("COMPANY TWO", organizations[1].OrganizationName);
            Assert.Equal("COMPANY THREE", organizations[2].OrganizationName);
            Assert.Equal("COMPANY FOUR", organizations[3].OrganizationName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllOrganizationsHierarchical()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository);

            // Act
            var organizations = await customerService.GetOrganizationsAsync(true);
            var childOrganizations = organizations[0].ChildOrganizations.ToList();

            // Assert
            Assert.Equal(3, organizations.Count); // only 3, because one organization is a child organization
            Assert.Null(organizations[0].ParentId);
            Assert.Null(organizations[1].ParentId);
            Assert.Null(organizations[2].ParentId);
            Assert.Equal(childOrganizations[0].ParentId, organizations[0].OrganizationId);  // organization one is parent of organization 3
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void DeleteOrganization()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository);

            // Act
            Guid customerFourId  = new("2C005777-ED56-43D9-9B1E-2B8112E67D10");
            await customerService.DeleteOrganizationAsync(customerFourId, Guid.Empty, false);
            var deletedOrganization = await customerService.GetOrganizationAsync(customerFourId);
            var organizations = await customerService.GetOrganizationsAsync(true);

            // Assert
            Assert.Equal(2, organizations.Count); // four organizations, but one is soft deleted
            Assert.Equal(customerFourId, deletedOrganization.OrganizationId);
            Assert.True(deletedOrganization.IsDeleted);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void EncryptDecryptMessage()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var customerService = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), customerRepository);

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
