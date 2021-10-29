using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class OrganizationServicesTests : OrganizationServicesBaseTest
    {
        public OrganizationServicesTests()
            : base(
                new DbContextOptionsBuilder<CustomerContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomerunittests.db")
                    .Options
            )
        {}

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetCompanyOne_CheckName()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository);

            // Act
            var organization = await organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);

            // Assert
            Assert.Equal("COMPANY ONE", organization.Name);
            Assert.Equal("My Way 1", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PutCompanyOne_null_values()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository);

            // Act
            var organization = await organizationServices.PutOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, "Mytos", null, null, null, null, null, null, null, null);

            // Assert 
            Assert.Equal("Mytos", organization.Name);
            Assert.Equal("", organization.OrganizationNumber);
            Assert.Equal(null, organization.ParentId);
            Assert.Equal(Guid.Empty, organization.PrimaryLocation);
            Assert.Equal(Guid.Empty, organization.UpdatedBy);
            Assert.Equal("", organization.Address.Street);
            Assert.Equal("", organization.Address.PostCode);
            Assert.Equal("", organization.Address.City);
            Assert.Equal("", organization.Address.Country);
            Assert.Equal("", organization.ContactPerson.FullName);
            Assert.Equal("", organization.ContactPerson.Email);
            Assert.Equal("", organization.ContactPerson.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PutCompanyOne_partial_null_values()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository);

            // Act
            var organization = await organizationServices.PutOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, "fullName", null, null);

            // Assert 
            Assert.Equal("name", organization.Name);
            Assert.Equal("", organization.OrganizationNumber);
            Assert.Equal(null, organization.ParentId);
            Assert.Equal(Guid.Empty, organization.PrimaryLocation);
            Assert.Equal(Guid.Empty, organization.UpdatedBy);
            Assert.Equal("street", organization.Address.Street);
            Assert.Equal("", organization.Address.PostCode);
            Assert.Equal("", organization.Address.City);
            Assert.Equal("", organization.Address.Country);
            Assert.Equal("fullName", organization.ContactPerson.FullName);
            Assert.Equal("", organization.ContactPerson.Email);
            Assert.Equal("", organization.ContactPerson.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PatchCompanyOne__null_values()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository);

            // Act
            var organization = await organizationServices.PatchOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, null, null, null, null, null, null, null, null, null);
            
            // Assert 
            Assert.Equal("COMPANY ONE", organization.Name);
            Assert.Equal("999888777", organization.OrganizationNumber);
            Assert.Equal(null, organization.ParentId);
            Assert.Equal(LOCATION_ONE_ID, organization.PrimaryLocation);
            Assert.Equal(Guid.Empty, organization.UpdatedBy);
            Assert.Equal("My Way 1", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
            Assert.Equal("JOHN DOE", organization.ContactPerson.FullName);
            Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
            Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PatchCompanyOne_partial_null_values()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository);

            // Act
            var organization = await organizationServices.PatchOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, "fullName", null, null);

            // Assert 
            Assert.Equal("name", organization.Name);
            Assert.Equal("999888777", organization.OrganizationNumber);
            Assert.Equal(null, organization.ParentId);
            Assert.Equal(LOCATION_ONE_ID, organization.PrimaryLocation);
            Assert.Equal(Guid.Empty, organization.UpdatedBy);
            Assert.Equal("street", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
            Assert.Equal("fullName", organization.ContactPerson.FullName);
            Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
            Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
        }
    }
}