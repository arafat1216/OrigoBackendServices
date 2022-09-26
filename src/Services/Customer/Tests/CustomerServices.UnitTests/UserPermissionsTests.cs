using AutoMapper;
using CustomerServices.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Infrastructure;
using CustomerServices.Mappings;
using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CustomerServices.Models;
using Xunit;

namespace CustomerServices.UnitTests;

public class UserPermissionsTests
{
    private readonly IMapper _mapper;
    private readonly IApiRequesterService _apiRequesterService;

    private DbContextOptions<CustomerContext> ContextOptions { get; }

    public UserPermissionsTests()
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
    public async Task GetUserAdmins_ForSystemAdmins_CheckCount()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationServices = Mock.Of<IOrganizationServices>();
        var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>(), _mapper, organizationServices);

        // Act
        var adminUsers = await userPermissionServices.GetUserAdminsAsync();

        // Assert
        Assert.Equal(2, adminUsers.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetUserAdmins_ForPartnerAdmins_CheckCount()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationServices = Mock.Of<IOrganizationServices>();
        var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>(), _mapper, organizationServices);

        // Act
        var adminUsers = await userPermissionServices.GetUserAdminsAsync(UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID);

        // Assert
        Assert.Equal(1, adminUsers.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetUserPermissions_ForPartnerAdmins_CheckAccessListWithoutPartnerId()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions, _apiRequesterService);
        var organizationServicesMock = new Mock<IOrganizationServices>();
        organizationServicesMock
            .Setup(os => os.GetOrganizationsAsync(false, true, UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID))
            .ReturnsAsync(await context.Organizations.ToListAsync());
        var userPermissionServices = new UserPermissionServices(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>(), _mapper, organizationServicesMock.Object);

        // Act
        var partnerAdminAccessList = await userPermissionServices.GetUserPermissionsAsync("partneradmin@doe.com");

        // Assert
        Assert.Equal(UnitTestDatabaseSeeder.TECHSTEP_CUSTOMER_ID, partnerAdminAccessList!.First().AccessList.First());
    }
}