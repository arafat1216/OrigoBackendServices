using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using Common.Infrastructure;
using Common.Interfaces;
using Common.Logging;
using CustomerServices.Email;
using CustomerServices.Email.Models;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Mappings;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests;

public class UserServicesTests
{
    private readonly IMapper _mapper;
    private readonly IApiRequesterService _apiRequesterService;

    private DbContextOptions<CustomerContext> ContextOptions { get; }

    public UserServicesTests()
    {
        ContextOptions = new DbContextOptionsBuilder<CustomerContext>()
            .UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
        new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddMaps(Assembly.GetAssembly(typeof(UserDTOProfile)));
        });
        _mapper = mappingConfig.CreateMapper();
        var apiRequesterServiceMock = new Mock<IApiRequesterService>();
        apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(UnitTestDatabaseSeeder.CALLER_ID);
        _apiRequesterService = apiRequesterServiceMock.Object;
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetUsersCount_ForCompanyOne()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var oktaMock = new Mock<IOktaServices>();
        const string OKTA_ID = "123";
        oktaMock.Setup(o => o.UserExistsInOktaAsync(OKTA_ID)).ReturnsAsync(true);
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID, true, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_THREE_ID, false, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        var users = await userServices.GetUsersCountAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, new[] { "SystemAdmin" });

        // Assert
        Assert.Equal(1, users!.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetUsersCount_ForCompanyTwo()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var oktaMock = new Mock<IOktaServices>();
        const string OKTA_ID = "123";
        oktaMock.Setup(o => o.UserExistsInOktaAsync(OKTA_ID)).ReturnsAsync(true);
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var users = await userServices.GetUsersCountAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, null, new[] { "SystemAdmin" });

        // Assert
        Assert.Equal(3, users?.NotOnboarded);
        Assert.Equal(0, users?.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAllUsers_ForCompanyOne()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var user = await userServices.GetUserAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID);

        // Assert
        Assert.Equal("Jane", user.FirstName);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserWithoutDepartment_CheckSavedExists()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        const string EMAIL_TEST_TEST = "email@test.test";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, "Test Firstname", "Testlastname",
            EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role");

        // Assert
        var newUserRead = await userServices.GetUserAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, newUser.Id);
        Assert.NotNull(newUserRead);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_WithFederatedUsers_ShouldNotCallOkta()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var oktaMock = new Mock<IOktaServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        const string EMAIL_TEST_TEST = "email@test.test";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, "Test Firstname", "Testlastname",
            EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role");

        // Assert
        oktaMock.Verify(
            okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_CustomerIsOnBoarded_ShouldInvitationMailToUser()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();

        var emailMock = new Mock<IEmailService>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, emailMock.Object);

        // Act
        const string EMAIL_TEST_TEST = "email@test.test";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_FOUR_ID, "Test Firstname", "Testlastname",
            EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, string.Empty, true);

        // Assert
        emailMock.Verify(email => email.InvitationEmailToUserAsync(It.IsAny<InvitationMail>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_UserShouldChangeStatusToInvited()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();

        var emailMock = new Mock<IEmailService>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, emailMock.Object);

        // Act
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_FOUR_ID, "Test Firstname", "Testlastname",
            "email@test.test", "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, null, true);

        // Assert
        Assert.Equal(2, newUser.UserStatus);
        Assert.Equal("Invited", newUser.UserStatusName);
        Assert.True(newUser.IsActiveState);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_NewUserWithAdminRoleAndCustomerHasAddToOkta_UserShouldChangeStatusToInvited()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionService = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(),
            Mock.Of<IMediator>(), _mapper, Mock.Of<IOrganizationServices>());

        var emailMock = new Mock<IEmailService>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionService, emailMock.Object);

        // Act
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_FOUR_ID, "Test Firstname", "Testlastname",
            "email@test.test", "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Admin", true);

        // Assert
        Assert.Equal(2, newUser.UserStatus);
        Assert.Equal("Invited", newUser.UserStatusName);
        Assert.True(newUser.IsActiveState);
        Assert.Equal("Admin", newUser.Role);

        var user = await organizationRepository.GetUserAsync(newUser.Id);
        Assert.Equal(UserStatus.Invited, user?.UserStatus);
        Assert.True(user?.IsActiveState);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_AddNewUserWithCustomerAddToOkta_ShouldCallOktaAndUserStatusNotInvited()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var oktaMock = new Mock<IOktaServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        const string EMAIL_TEST_TEST = "email@test.test";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, "Test Firstname", "Testlastname",
            EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role", true);

        // Assert
        oktaMock.Verify(
            okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        Assert.Equal("NotInvited", newUser.UserStatusName);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_AddNewUserWithCustomerAddToOktaButUserNotAdd_ShouldNotCallOkta()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var oktaMock = new Mock<IOktaServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        const string EMAIL_TEST_TEST = "email@test.test";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, "Test Firstname", "Testlastname",
            EMAIL_TEST_TEST, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role", true, true);

        // Assert
        oktaMock.Verify(
            okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_ReActivatedUserWithCustomerAddToOkta_ShouldCallOktaAndGetUserStatusNotInvited()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var oktaMock = new Mock<IOktaServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var deleteUser = await userServices.DeleteUserAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, UnitTestDatabaseSeeder.USER_ONE_ID, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var reActiveUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, "Test Firstname", "Testlastname",
            deleteUser.Email, "+4741676767", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role");

        // Assert
        oktaMock.Verify(
            okta => okta.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        Assert.Equal("NotInvited", reActiveUser.UserStatusName);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignUserPermissions_DepartmentManager_WithEmptyAccessList()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(),
            Mock.Of<IMediator>(), _mapper, Mock.Of<IOrganizationServices>());

        // Act
        var permission = await userPermissionServices.AssignUserPermissionsAsync("jane@doe.com", "DepartmentManager",
            new List<Guid>(), UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        //Assert
        Assert.Null(permission);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserAsManager_CheckManagedDepartmentCount()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(),
            Mock.Of<IMediator>(), _mapper, Mock.Of<IOrganizationServices>());
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        await userPermissionServices.AssignUserPermissionsAsync("jane@doe.com", "DepartmentManager",
            new List<Guid> { UnitTestDatabaseSeeder.CUSTOMER_ONE_ID }, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        //Assert
        var permission = await context.UserPermissions.FirstOrDefaultAsync(u => u.User.Email == "jane@doe.com");
        Assert.Equal("jane@doe.com", permission?.User.Email);

        //Act
        await userServices.AssignManagerToDepartment(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID, UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        // Assert
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == UnitTestDatabaseSeeder.USER_ONE_ID);
        Assert.Equal(1, user!.ManagesDepartments.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserAsManager_CheckManagedDepartmentCount_MissingDepartmentRoleException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act & Assert
        await Assert.ThrowsAsync<MissingRolePermissionsException>(() =>
            userServices.AssignManagerToDepartment(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID, UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID, UnitTestDatabaseSeeder.EMPTY_CALLER_ID));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SetUserActive()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var oktaMock = new Mock<IOktaServices>();
        const string OKTA_ID = "123";
        oktaMock.Setup(o => o.UserExistsInOktaAsync(OKTA_ID)).ReturnsAsync(true);
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID, true, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var user = await userServices.GetUserAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID);

        // Assert
        Assert.Equal("123", user.OktaUserId);
        Assert.Equal(UserStatus.Activated, user.UserStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task DeactivateUser()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var oktaMock = new Mock<IOktaServices>();
        const string OKTA_ID = "1234";
        oktaMock.Setup(o => o.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(OKTA_ID);
        oktaMock.Setup(o => o.UserExistsInOktaAsync(It.IsAny<string>())).ReturnsAsync(true);
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());
        await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_THREE_ID, true,
            UnitTestDatabaseSeeder.EMPTY_CALLER_ID); // Activate user

        // Act
        var user = await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_THREE_ID, false, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        // Assert
        oktaMock.Verify(mock => mock.RemoveUserFromGroupAsync(It.IsAny<string>()), Times.Once());
        Assert.Equal("Deactivated", user.UserStatusName);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task ReactivateUser()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var oktaMock = new Mock<IOktaServices>();
        oktaMock.Setup(o => o.UserExistsInOktaAsync(It.IsAny<string>())).ReturnsAsync(true);
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository, oktaMock.Object,
            _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_THREE_ID, false, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        var user = await userServices.SetUserActiveStatusAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_THREE_ID, true, UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        // Assert
        oktaMock.Verify(
            mock => mock.AddOktaUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once());
        Assert.Equal("Activated", user.UserStatusName);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAllUsers_AddAUser_ListCountPlusOne()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var newUser = await userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, "TEST", "TEST", "hello@mail.com",
            "+479898989", "hhhh", new UserPreference("EN", UnitTestDatabaseSeeder.EMPTY_CALLER_ID), UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role");
        IList<int>? status = new List<int> { (int)UserStatus.Activated };
        string[]? role = { "admin" };
        Guid[]? assignedToDepartment = { UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID };

        var user = await userServices.GetAllUsersAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null, status, new CancellationToken());

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
    public async Task GetAllUsers_OffboardUsers()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var user = await userServices.GetAllUsersAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, null, null, new List<int> { 6,7,8 }, new CancellationToken());

        Assert.Equal(1, user.Items.Count);
        Assert.Contains("Jhon", user.Items[0].FirstName);
        Assert.Contains("Cena", user.Items[0].LastName);
        Assert.Contains("jhoncena@wwe.com", user.Items[0].Email);
        Assert.Contains("+4790000001", user.Items[0].MobileNumber);
        Assert.Equal(DateTime.UtcNow.Date, user.Items[0].LastWorkingDay!.Value.Date);
        Assert.IsType<PagedModel<UserDTO>>(user);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomer_CustomerNotFoundException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        //Act
        const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

        //Assert
        await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
            userServices.AddUserForCustomerAsync(new Guid(NOT_VALID_CUSTOMER_ID), "TEST", "TEST", "hello@mail.com",
                "+479898989", "90909090", new UserPreference("EN", UnitTestDatabaseSeeder.EMPTY_CALLER_ID), UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role"));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignManager_CustomerNotFound_ThrowsCustomerNotFoundException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        //Act
        const string NOT_VALID_CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

        //Assert
        await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
            userServices.AssignManagerToDepartment(new Guid(NOT_VALID_CUSTOMER_ID), UnitTestDatabaseSeeder.USER_ONE_ID, UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID,
                UnitTestDatabaseSeeder.EMPTY_CALLER_ID));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignManager_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        //Act
        const string NOT_VALID_USER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

        //Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            userServices.AssignManagerToDepartment(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, new Guid(NOT_VALID_USER_ID), UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID,
                UnitTestDatabaseSeeder.EMPTY_CALLER_ID));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignManager_DepartmentNotFound_ThrowsDepartmentNotFoundException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var customerRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), customerRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        //Act
        const string NOT_VALID_DEPARTMENT_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";

        //Assert
        await Assert.ThrowsAsync<DepartmentNotFoundException>(() =>
            userServices.AssignManagerToDepartment(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.USER_ONE_ID, new Guid(NOT_VALID_DEPARTMENT_ID),
                UnitTestDatabaseSeeder.EMPTY_CALLER_ID));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomerAsync_WithExistingMail_ShouldThrowUserNameIsInUseException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());
        // Act
        const string EMAIL_THAT_EXIST = "john@doe.com";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        // Assert
        await Assert.ThrowsAsync<UserNameIsInUseException>(() => userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            "Test Firstname", "Testlastname", EMAIL_THAT_EXIST, "+479000000", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID,
            "Role"));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task
        AddUserForCustomerAsync_AddingUserWithExistingMail_ShouldThrowExceptionAndSaveChangesShouldNotBeCalled()
    {
        // Arrange
        var mockRepository = new Mock<IOrganizationRepository>();

        var organizationId = Guid.NewGuid();
        var preferences = new OrganizationPreferences(organizationId, Guid.Empty, null, null, null, false, "", 1);
        var organization = new Organization(organizationId, null, "C1", "1", new Address(), new ContactPerson(),
            preferences, new Location("A", "D", "A1", "A2", "P", "C", "CO"), null, true, 1, null, "");

        mockRepository.Setup(o => o.GetOrganizationAsync(organizationId,
            It.IsAny<Expression<Func<Organization, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),
            It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organization);
        mockRepository.Setup(o => o.GetOrganizationPreferencesAsync(organizationId)).ReturnsAsync(preferences);

        const string EMAIL_THAT_EXIST = "john@doe.com";
        const string MOBILE_NUMBER_THAT_EXIST = "+4798888811";
        var existingUser = new User(organization, Guid.NewGuid(), "John", "Doe", EMAIL_THAT_EXIST,
            MOBILE_NUMBER_THAT_EXIST, "emp123", new UserPreference("no", UnitTestDatabaseSeeder.CALLER_ID), UnitTestDatabaseSeeder.CALLER_ID);

        mockRepository.Setup(u => u.GetUserByUserName(EMAIL_THAT_EXIST)).ReturnsAsync(existingUser);
        mockRepository.Setup(u => u.GetUserByMobileNumber(MOBILE_NUMBER_THAT_EXIST, organizationId)).ReturnsAsync(existingUser);


        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), mockRepository.Object,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);

        // Act and assert
        await Assert.ThrowsAsync<UserNameIsInUseException>(() => userServices.AddUserForCustomerAsync(organizationId,
            "Test Firstname", "Testlastname", EMAIL_THAT_EXIST, "+479000000", "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID,
            "Role"));
        mockRepository.Verify(r => r.SaveEntitiesAsync(CancellationToken.None), Times.Exactly(0));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddUserForCustomerAsync_WithExistingPhoneNumber_ShouldThrowInvalidPhoneNumberException()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());
        // Act
        const string PHONE_NUMBER_THAT_EXIST = "+4799999999";
        var userPref = new UserPreference("NO", UnitTestDatabaseSeeder.EMPTY_CALLER_ID);
        // Assert
        await Assert.ThrowsAsync<InvalidPhoneNumberException>(() =>
            userServices.AddUserForCustomerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, "Test Firstname", "Testlastname", "mail@testmail.com",
                PHONE_NUMBER_THAT_EXIST, "43435435", userPref, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, "Role"));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganizationUsersCountAsync_CountForSystemAdminRole()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        var count = await organizationRepository.GetOrganizationUsersCountAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null,
            new[] { "SystemAdmin" });

        Assert.Equal(2, count.NotOnboarded);
        Assert.Equal(1, count.Count);
        Assert.Equal(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, count.OrganizationId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganizationUsersCountAsync_OnlyCountForDepartments()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        var count = await organizationRepository.GetOrganizationUsersCountAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            new[] { UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID, UnitTestDatabaseSeeder.DEPARTMENT_TWO_ID }, null);

        Assert.Equal(1, count.NotOnboarded);
        Assert.Equal(1, count.Count);
        Assert.Equal(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, count.OrganizationId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganizationUsersCountAsync_NoRoleAndNoDepartments_ShouldReturnNull()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        var organizationUserCount =
            await organizationRepository.GetOrganizationUsersCountAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null);

        Assert.Null(organizationUserCount);
    }

    
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task CompleteOnboardingAsync()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var userPermissionServices = Mock.Of<IUserPermissionServices>();
        var userServices = new UserServices(Mock.Of<ILogger<UserServices>>(), organizationRepository,
            Mock.Of<IOktaServices>(), _mapper, userPermissionServices, Mock.Of<IEmailService>());

        // Act
        var user = await userServices.CompleteOnboardingAsync(UnitTestDatabaseSeeder.CUSTOMER_TWO_ID, UnitTestDatabaseSeeder.USER_SEVEN_ID);

        // Assert
        Assert.Equal(0, user.UserStatus);
    }
}