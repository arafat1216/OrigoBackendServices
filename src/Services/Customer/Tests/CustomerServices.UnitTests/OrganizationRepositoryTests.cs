using System;
using System.Threading.Tasks;
using Common.Logging;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class OrganizationRepositoryTests
    {
        private DbContextOptions<CustomerContext> ContextOptions { get; }

        public OrganizationRepositoryTests()
        {
            ContextOptions = new DbContextOptionsBuilder<CustomerContext>()
                .UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
            new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
        }

        [Fact]
        public async Task GetUserByMobileNumber_WithNumberFromOtherOrganization_ShouldNotBeFound()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            const string USED_MOBILE_NUMBER_IN_CUSTOMER_ONE = "+4799999999";
            var notFoundUser = await organizationRepository.GetUserByMobileNumber(USED_MOBILE_NUMBER_IN_CUSTOMER_ONE, UnitTestDatabaseSeeder.CUSTOMER_THREE_ID);
            Assert.Null(notFoundUser);
        }

        [Fact]
        public async Task GetUserByMobileNumber_WithNumberFromSameOrganization_ShouldBeFound()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            const string USED_MOBILE_NUMBER_IN_CUSTOMER_ONE = "+4799999999";
            var notFoundUser = await organizationRepository.GetUserByMobileNumber(USED_MOBILE_NUMBER_IN_CUSTOMER_ONE, UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
            Assert.Equal(notFoundUser!.MobileNumber, USED_MOBILE_NUMBER_IN_CUSTOMER_ONE);
        }
    }
}
