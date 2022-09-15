using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using Common.Logging;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests;

public class DepartmentServicesTests : OrganizationServicesBaseTest
{
    private IMapper _mapper;

    public DepartmentServicesTests() : base(
                new DbContextOptionsBuilder<CustomerContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitedepartmentsunittests.db").Options
            )
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(DepartmentsServices))); });
            _mapper = mappingConfig.CreateMapper();
        }
    }

    [Fact]
    public async Task UpdateDepartment_WithNoDepartmentManagers_IsRemoved()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        const string DEPARTMENT_NAME = "DEPT_NAME";
        const string COST_CENTER_NAME = "CC1";
        const string DEPARTMENT_DESCRIPTION = "DEPT_DEC";
        var organizationRepositoryMock = new Mock<IOrganizationRepository>();
        var organization = new Organization(customerId, null, "C1", "1", new Address(),
            new ContactPerson(), new OrganizationPreferences(customerId, Guid.Empty, null, null, null, false, "", 1), new Location("A", "D", "A1", "A2", "P", "C", "CO", RecipientType.Organization), null, true,1, null, null, "", false);
        organizationRepositoryMock
            .Setup(o => o.GetOrganizationAsync(customerId, It.IsAny<Expression<Func<Organization, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organization);
        var existingDepartment = new Department(DEPARTMENT_NAME, COST_CENTER_NAME, DEPARTMENT_DESCRIPTION, organization, departmentId, Guid.Empty);
        existingDepartment.AddDepartmentManager(new User(organization, Guid.NewGuid(), "Ola", "Nordmann", "ola.nordmann@test.test", "+4791111111", "123", new UserPreference("lang", Guid.Empty), Guid.Empty), Guid.Empty);
        organizationRepositoryMock.Setup(o => o.GetDepartmentsAsync(customerId)).ReturnsAsync(new List<Department>
        {
            existingDepartment
        });
        var userPermissionServicesMock = new Mock<UserPermissionServices>();
        var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), organizationRepositoryMock.Object, _mapper, userPermissionServicesMock.Object);

        // Act
        var department = await departmentService.UpdateDepartmentAsync(customerId, departmentId, null, DEPARTMENT_NAME, COST_CENTER_NAME, DEPARTMENT_DESCRIPTION, new List<Guid>(),
            Guid.Empty);

        // Assert
        Assert.Empty(department.ManagedBy);
    }
    [Fact]
    public async Task UpdateDepartment_AddDuplicatedManagers_()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var departmentServices = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), organizationRepository, _mapper, Mock.Of<IUserPermissionServices>());

        var departments = await context.Departments.Include(m => m.Managers).FirstOrDefaultAsync(c => c.ExternalDepartmentId == DEPARTMENT_TWO_ID);
        Assert.Equal(2, departments?.Managers.Count);
        Assert.Collection(departments?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );

        var managers = new List<Guid>{USER_FOUR_ID, USER_FIVE_ID, USER_FIVE_ID, USER_FOUR_ID};

        // Act
        var department = await departmentServices.UpdateDepartmentAsync(CUSTOMER_ONE_ID, DEPARTMENT_TWO_ID, null, null, null, null, managers,
            Guid.Empty);

        // Assert
        Assert.Equal(2, department.ManagedBy.Count);
        Assert.Collection(department?.ManagedBy,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );

        
    }
    public async Task UpdateDepartmentAsync_AddingManagers()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var departmentServices = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), organizationRepository, _mapper, Mock.Of<IUserPermissionServices>());

        var departments = await context.Departments.Include(m => m.Managers).FirstOrDefaultAsync(c => c.ExternalDepartmentId == DEPARTMENT_TWO_ID);
        Assert.Equal(2, departments?.Managers.Count);
        Assert.Collection(departments?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );

        var managers = new List<Guid> { USER_FOUR_ID, USER_FIVE_ID, USER_FIVE_ID, USER_FOUR_ID };

        // Act
        var department = await departmentServices.UpdateDepartmentAsync(CUSTOMER_ONE_ID, DEPARTMENT_TWO_ID, null, null, null, null, managers,
            Guid.Empty);

        // Assert
        Assert.Equal(2, department.ManagedBy.Count);
        Assert.Collection(department?.ManagedBy,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );
    }
    [Fact]
    public async Task GetDepartmentsAsync_IncludingManagers()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var departmentServices = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), organizationRepository, _mapper, Mock.Of<IUserPermissionServices>());

        var department = await context.Departments.Include(m => m.Managers).FirstOrDefaultAsync(c => c.ExternalDepartmentId == DEPARTMENT_TWO_ID);
        Assert.Equal(2, department?.Managers.Count);
        Assert.Collection(department?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );

        //Act
        var departments = await organizationRepository.GetDepartmentsAsync(CUSTOMER_ONE_ID);
        var departmentTwo = departments.Where(a=>a.ExternalDepartmentId == DEPARTMENT_TWO_ID).FirstOrDefault();

        //Assert
        Assert.Equal(2, departmentTwo?.Managers.Count);
        Assert.Collection(departmentTwo?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );
    }
    [Fact]
    public async Task GetDepartmentAsync_IncludingManagers()
    {
        // Arrange
        await using var context = new CustomerContext(ContextOptions);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var departmentServices = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), organizationRepository, _mapper, Mock.Of<IUserPermissionServices>());

        var department = await context.Departments.Include(m => m.Managers).FirstOrDefaultAsync(c => c.ExternalDepartmentId == DEPARTMENT_TWO_ID);
        Assert.Equal(2, department?.Managers.Count);
        Assert.Collection(department?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );

        //Act
        var departmentTwo = await organizationRepository.GetDepartmentAsync(CUSTOMER_ONE_ID, DEPARTMENT_TWO_ID);

        //Assert
        Assert.Equal(2, departmentTwo?.Managers.Count);
        Assert.Collection(departmentTwo?.Managers,
             item => Assert.Equal(USER_FOUR_ID, item.UserId),
             item => Assert.Equal(USER_FIVE_ID, item.UserId)
         );
    }
}