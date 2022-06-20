using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests;

public class DepartmentServicesTests
{
    private IMapper _mapper;

    public DepartmentServicesTests()
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
        var organization = new Organization(customerId, Guid.Empty, null, "C1", "1", new Address(),
            new ContactPerson(), new OrganizationPreferences(customerId, Guid.Empty, null, null, null, false, "", 1), new Location(Guid.Empty, "A", "D", "A1", "A2", "P", "C", "CO", RecipientType.Organization), null, true, false);
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
}