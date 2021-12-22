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
using System.Reflection;
using AutoMapper;
using CustomerServices.Mappings;
using CustomerServices.ServiceModels;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class UserServicesTests : OrganizationServicesBaseTest
    {
        private static IMapper _mapper;

        public UserServicesTests() : base(new DbContextOptionsBuilder<CustomerContext>()
            // ReSharper disable once StringLiteralTypo
            .UseSqlite("Data Source=sqliteusersunittests.db").Options)
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(UserDTOProfile)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetUsersCount_ForCompanyOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

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
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

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
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            // Act
            const string EMAIL_TEST_TEST = "email@test.test";
            var userPref = new Models.UserPreference("NO");
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4799999999", "43435435", userPref);

            // Assert
            var newUserRead = await userServices.GetUserAsync(CUSTOMER_ONE_ID, newUser.Id);
            Assert.NotNull(newUserRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserAsManager_CheckManagedDepartmentCount()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

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
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object, _mapper, userPermissionServices);

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
            oktaMock.Setup(o => o.UserExistsInOktaAsync(It.IsAny<string>())).ReturnsAsync(true);
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object, _mapper, userPermissionServices);
            await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, true); // Activate user

            // Act
            var user = await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, false);

            // Assert
            oktaMock.Verify(mock => mock.RemoveUserFromGroup(It.IsAny<string>()), Times.Once());
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
            oktaMock.Setup(o => o.UserExistsInOktaAsync(It.IsAny<string>())).ReturnsAsync(true);
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object, _mapper, userPermissionServices);

            // Act
            await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, false);
            var user = await userServices.SetUserActiveStatus(CUSTOMER_ONE_ID, USER_ONE_ID, true);

            // Assert
            oktaMock.Verify(mock => mock.AddUserToGroup(It.IsAny<string>()), Times.Once());
            Assert.True(user.IsActive);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAllUsers_AddAUser_ListCountPlusOne()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            // Act
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "TEST", "TEST", "hello@mail.com", "+479898989", "hhhh", new UserPreference("EN"));
            var user = await userServices.GetAllUsersAsync(CUSTOMER_ONE_ID);

            Assert.Equal(3, user.Count);
            Assert.Contains("TEST", user[2].FirstName);
            Assert.Contains("TEST", user[2].LastName);
            Assert.Contains("hello@mail.com", user[2].Email);
            Assert.Contains("+479898989", user[2].MobileNumber);
            Assert.Contains("hhhh", user[2].EmployeeId);
            Assert.IsType<List<UserDTO>>(user);

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async System.Threading.Tasks.Task AddUserForCustomer_CustomerNotFoundException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            //Act
            const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            
            //Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => userServices.AddUserForCustomerAsync(new Guid(NOT_VALID_CUSTOMER_ID), "TEST", "TEST", "hello@mail.com", "+479898989", "90909090", new UserPreference("EN")));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AssignManager_UserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            //Act
            const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => userServices.AssignManagerToDepartment(new Guid(NOT_VALID_CUSTOMER_ID), USER_ONE_ID, DEPARTMENT_ONE_ID));

        }
    }
}
