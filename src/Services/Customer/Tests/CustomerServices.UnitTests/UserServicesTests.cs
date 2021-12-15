﻿using Common.Logging;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class UserServicesTests : OrganizationServicesBaseTest
    {
        public UserServicesTests() : base(new DbContextOptionsBuilder<CustomerContext>()
            // ReSharper disable once StringLiteralTypo
            .UseSqlite("Data Source=sqliteusersunittests.db").Options)
        {
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetUsersCount_ForCompanyOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            // Act
            int user = await userServices.GetUsersCountAsync(CUSTOMER_ONE_ID);

            // Assert
            Assert.Equal(2, user);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllUsers_ForCompanyOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

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
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>());

            // Act
            const string EMAIL_TEST_TEST = "email@test.test";
            var userPref = new Models.UserPreference("NO");
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4799999999", "43435435", userPref);

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
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>());

            // Act
            await userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID);

            // Assert
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == USER_ONE_ID);
            Assert.Equal(1, user.ManagesDepartments.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SetUserActive()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var oktaMock = new Mock<IOktaServices>();
            const string OKTA_ID = "1234";
            oktaMock.Setup(o => o.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<string>())).ReturnsAsync(OKTA_ID);
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object);

            // Act
            await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, true);
            var user = await userServices.GetUserAsync(CUSTOMER_ONE_ID, USER_ONE_ID);

            // Assert
            Assert.Equal("1234", user.OktaUserId);
            Assert.True(user.IsActive);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void DeactivateUser()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var oktaMock = new Mock<IOktaServices>();
            const string OKTA_ID = "1234";
            oktaMock.Setup(o => o.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(OKTA_ID);
            oktaMock.Setup(o => o.UserExistsInOktaAsync(OKTA_ID)).ReturnsAsync(true);
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object);
            await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, true); // Activate user

            // Act
            var user = await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, false);

            // Assert
            Assert.Equal("1234", user.OktaUserId); // Should not be changed, as user can be reactivated later
            Assert.False(user.IsActive);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ReactivateUser()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var oktaMock = new Mock<IOktaServices>();
            const string OKTA_ID = "1234";
            oktaMock.Setup(o => o.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<string>())).ReturnsAsync(OKTA_ID);
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object);

            // Act
            await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, false);
            var user = await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, true);

            // Assert
            Assert.Equal("1234", user.OktaUserId); 
            Assert.True(user.IsActive);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UnassignDepartment_ShouldReturnUserWithTypeUser()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            //Act
            var user = await userServices.UnassignDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID);

            //Assert
            Assert.NotNull(user);
            Assert.IsType<User>(user);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllUsers_AddAUser_ListCountPlussOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            // Act
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "TEST", "TEST", "hello@mail.com", "+479898989", "hhhh", new UserPreference("EN"));
            var user = await userServices.GetAllUsersAsync(CUSTOMER_ONE_ID);

            Assert.Equal(3, user.Count);
            Assert.Contains("TEST", user[2].FirstName);
            Assert.Contains("TEST", user[2].LastName);
            Assert.Contains("hello@mail.com", user[2].Email);
            Assert.Contains("+479898989", user[2].MobileNumber);
            Assert.Contains("hhhh", user[2].EmployeeId);
            Assert.IsType<List<User>>(user);

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async System.Threading.Tasks.Task AddUserForCustomer_CustomerNotFoundExeption()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            //Act
            const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            
            //Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => userServices.AddUserForCustomerAsync(new Guid(NOT_VALID_CUSTOMER_ID), "TEST", "TEST", "hello@mail.com", "+479898989", "90909090", new UserPreference("EN")));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AssignManager_UserNotFound_ThrowsUserNotFoundExeption()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            //Act
            const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => userServices.AssignManagerToDepartment(new Guid(NOT_VALID_CUSTOMER_ID), USER_ONE_ID, DEPARTMENT_ONE_ID));

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UnassignManagerFromDepartment_UserWithNoDepartment()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>());

            //Act
            var userRemoveDepartment = await userServices.UnassignDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID);

            //Assert
            Assert.Equal(0, userRemoveDepartment.Departments.Count);
        }

    }
}
