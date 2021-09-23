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

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserWithoutDepartment_CheckSavedExists()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository);

            // Act
            const string EMAIL_TEST_TEST = "email@test.test";
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4799999999", "43435435");

            // Assert
            var newUserRead = await userServices.GetUserAsync(CUSTOMER_ONE_ID, newUser.UserId);
            Assert.NotNull(newUserRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserAsManager_CheckManagedDepartmentCount()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository);

            // Act
            await userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID);

            // Assert
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == USER_ONE_ID);
            Assert.Equal(1, user.ManagesDepartments.Count);
        }
    }
}