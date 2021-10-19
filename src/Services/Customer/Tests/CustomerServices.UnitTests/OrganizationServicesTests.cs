using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
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
    }
}