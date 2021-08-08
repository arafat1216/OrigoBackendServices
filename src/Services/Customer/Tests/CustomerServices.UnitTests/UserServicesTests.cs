using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class UserServicesTests : CustomerServicesBaseTest
    {
        public UserServicesTests() : base(new DbContextOptionsBuilder<CustomerContext>()
            // ReSharper disable once StringLiteralTypo
            .UseSqlite("Data Source=sqliteusersunittests.db").Options)
        {
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllUsers_ForCompanyOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository);

            // Act
            var user = await userServices.GetUserAsync(CUSTOMER_ONE_ID, USER_ONE_ID);

            // Assert
            Assert.Equal("Jane", user.FirstName);
        }
    }
}