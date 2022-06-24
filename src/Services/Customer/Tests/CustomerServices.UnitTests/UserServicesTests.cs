using Common.Logging;
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
using System.Threading.Tasks;
using AutoMapper;
using CustomerServices.Mappings;
using CustomerServices.ServiceModels;
using Xunit;
using CustomerServices.Infrastructure.Context;
using Common.Interfaces;
using Common.Enums;

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
            var oktaMock = new Mock<IOktaServices>();
            const string OKTA_ID = "1234";
            oktaMock.Setup(o => o.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<string>())).ReturnsAsync(OKTA_ID);
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);
            
            // Act
            await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, true, EMPTY_CALLER_ID);
            await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_THREE_ID, false, EMPTY_CALLER_ID);

            int users = await userServices.GetUsersCountAsync(CUSTOMER_ONE_ID, null, new[] {"SystemAdmin"});

            // Assert
            Assert.Equal(1, users);
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
            var userPref = new Models.UserPreference("NO", EMPTY_CALLER_ID);
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, EMPTY_CALLER_ID, "Role");

            // Assert
            var newUserRead = await userServices.GetUserAsync(CUSTOMER_ONE_ID, newUser.Id);
            Assert.NotNull(newUserRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserForCustomer_WithFederatedUsers_ShouldNotCallOkta()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var oktaMock = new Mock<IOktaServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object, _mapper, userPermissionServices);

            // Act
            const string EMAIL_TEST_TEST = "email@test.test";
            var userPref = new Models.UserPreference("NO", EMPTY_CALLER_ID);
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, EMPTY_CALLER_ID, "Role");

            // Assert
            oktaMock.Verify(okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserForCustomer_WithAddToOkta_ShouldCallOkta()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var oktaMock = new Mock<IOktaServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object, _mapper, userPermissionServices);

            // Act
            const string EMAIL_TEST_TEST = "email@test.test";
            var userPref = new Models.UserPreference("NO", EMPTY_CALLER_ID);
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_TWO_ID, "Test Firstname", "Testlastname", EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, EMPTY_CALLER_ID, "Role");

            // Assert
            oktaMock.Verify(okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AssignUserPermissions_DepartmentManager_WithEmptyAccessList()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>(), _mapper);

            // Act
            var permission = await userPermissionServices.AssignUserPermissionsAsync("jane@doe.com", "DepartmentManager", new List<Guid>(), EMPTY_CALLER_ID);

            //Assert
            Assert.Null(permission);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddUserAsManager_CheckManagedDepartmentCount()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>(), _mapper);
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            // Act
            await userPermissionServices.AssignUserPermissionsAsync("jane@doe.com", "DepartmentManager", new List<Guid> { CUSTOMER_ONE_ID }, EMPTY_CALLER_ID);

            //Assert
            var permission = await context.UserPermissions.FirstOrDefaultAsync(u => u.User.Email == "jane@doe.com");
            Assert.Equal("jane@doe.com", permission?.User.Email);

            //Act
            await userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID, EMPTY_CALLER_ID);

            // Assert
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == USER_ONE_ID);
            Assert.Equal(1, user!.ManagesDepartments.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddUserAsManager_CheckManagedDepartmentCount_MissingDepartmentRoleException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            // Act & Assert
            await Assert.ThrowsAsync<MissingRolePermissionsException>(() => userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, DEPARTMENT_ONE_ID, EMPTY_CALLER_ID));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task SetUserActive()
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
            await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, true, EMPTY_CALLER_ID);
            var user = await userServices.GetUserAsync(CUSTOMER_ONE_ID, USER_ONE_ID);

            // Assert
            Assert.Equal("1234", user.OktaUserId);
            Assert.Equal(Common.Enums.UserStatus.Activated, user.UserStatus);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeactivateUser()
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
            await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, true,EMPTY_CALLER_ID); // Activate user

            // Act
            var user = await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, false, EMPTY_CALLER_ID);

            // Assert
            oktaMock.Verify(mock => mock.RemoveUserFromGroupAsync(It.IsAny<string>()), Times.Once());
            Assert.Equal("Deactivated", user.UserStatusName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task ReactivateUser()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var oktaMock = new Mock<IOktaServices>();
            oktaMock.Setup(o => o.UserExistsInOktaAsync(It.IsAny<string>())).ReturnsAsync(true);
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object, _mapper, userPermissionServices);

            // Act
            await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, false, EMPTY_CALLER_ID);
            var user = await userServices.SetUserActiveStatusAsync(CUSTOMER_ONE_ID, USER_ONE_ID, true, EMPTY_CALLER_ID);

            // Assert
            oktaMock.Verify(mock => mock.AddUserToGroup(It.IsAny<string>()), Times.Once());
            Assert.Equal("Activated", user.UserStatusName);
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
            var newUser = await userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "TEST", "TEST", "hello@mail.com", "+479898989", "hhhh", new UserPreference("EN", EMPTY_CALLER_ID), EMPTY_CALLER_ID,"Role");
            IList<int>? status = new List<int>{(int)UserStatus.Activated };
            string[]? role = new string[] {"admin"};
            Guid[]? assignedToDepartment = new Guid[]{ DEPARTMENT_ONE_ID};

            var user = await userServices.GetAllUsersAsync(CUSTOMER_ONE_ID,null, null,status, new System.Threading.CancellationToken());

            Assert.Equal(1, user.Items.Count);
            Assert.Contains("Gordon", user.Items[0].FirstName);
            Assert.Contains("Freeman", user.Items[0].LastName);
            Assert.Contains("gordon@freeman.com", user.Items[0].Email);
            Assert.Contains("+4755555555", user.Items[0].MobileNumber);
            Assert.Contains("DH-101", user.Items[0].EmployeeId);
            Assert.IsType<PagedModel<UserDTO>>(user);

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
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => userServices.AddUserForCustomerAsync(new Guid(NOT_VALID_CUSTOMER_ID), "TEST", "TEST", "hello@mail.com", "+479898989", "90909090", new UserPreference("EN", EMPTY_CALLER_ID), EMPTY_CALLER_ID,"Role"));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AssignManager_CustomerNotFound_ThrowsCustomerNotFoundException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            //Act
            const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

            //Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => userServices.AssignManagerToDepartment(new Guid(NOT_VALID_CUSTOMER_ID), USER_ONE_ID, DEPARTMENT_ONE_ID, EMPTY_CALLER_ID));
            
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
            const string NOT_VALID_USER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, new Guid(NOT_VALID_USER_ID), DEPARTMENT_ONE_ID, EMPTY_CALLER_ID));

        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AssignManager_DepartmentNotFound_ThrowsDepartmentNotFoundException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);

            //Act
            const string NOT_VALID_DEPARTMENT_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

            //Assert
            await Assert.ThrowsAsync<DepartmentNotFoundException>(() => userServices.AssignManagerToDepartment(CUSTOMER_ONE_ID, USER_ONE_ID, new Guid(NOT_VALID_DEPARTMENT_ID), EMPTY_CALLER_ID));

        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserForCustomerAsync_WithExistingMail_ShouldThrowInvalidEmailException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);
            // Act
            const string EMAIL_THAT_EXIST = "john@doe.com";
            var userPref = new Models.UserPreference("NO", EMPTY_CALLER_ID);

            // Assert
            await Assert.ThrowsAsync<UserNameIsInUseException>(() => userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", EMAIL_THAT_EXIST, "+479000000", "43435435", userPref, EMPTY_CALLER_ID, "Role"));

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddUserForCustomerAsync_WithExistingPhoneNumber_ShouldThrowInvalidPhoneNumberException()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var userPermissionServices = Mock.Of<IUserPermissionServices>();
            var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, Mock.Of<IOktaServices>(), _mapper, userPermissionServices);
            // Act
            const string PHONE_NUMBER_THAT_EXIST = "+4799999999";
            var userPref = new Models.UserPreference("NO", EMPTY_CALLER_ID);
            // Assert
            await Assert.ThrowsAsync<InvalidPhoneNumberException>(() => userServices.AddUserForCustomerAsync(CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", "mail@testmail.com", PHONE_NUMBER_THAT_EXIST, "43435435", userPref, EMPTY_CALLER_ID,"Role"));
        }
    }
}
