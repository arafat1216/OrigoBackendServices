using System.Net.Http.Json;
using Common.Enums;
using Common.Extensions;
using Customer.API.CustomerDatasyncModels;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using CustomerServices.ServiceModels;
using Customer.API.ViewModels;
using Customer.API.WriteModels;

namespace Customer.API.IntegrationTests.Controllers;

public class CustomerDatasyncDepartmentsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{
    private readonly HttpClient _httpClient;
    private readonly Guid _customerId;
    private const string RequestUri = "api/v1/customer-datasync/departments";
    private const string EmployeeRequestUri = "api/v1/customer-datasync/users";

    private readonly CustomerWebApplicationFactory<Startup> _factory;

    public CustomerDatasyncDepartmentsControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
    {
        _httpClient = factory.CreateDefaultClient();
        _customerId = factory.ORGANIZATION_ID;
        _factory = factory;
        _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.PubsubUserId().ToString());
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task CreateDepartment_WithValidUserData_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department1",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
            
        var customerId = _factory.ORGANIZATION_ID;

        // Act
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();

        // Assert
        response = await httpClient.GetAsync($"{RequestUri}/{department.DepartmentId}/organizations/{customerId}");
        department = await response.Content.ReadFromJsonAsync<Department>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newDepartmentEvent.Name, department.Name);
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task UpdateDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department1",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
            
        var customerId = _factory.ORGANIZATION_ID;

        // Act
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        UpdateDepartmentEvent updateDepartmentEvent = new ()
        {
            Name = "Updated Department1",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>(),
            DepartmentId = department.DepartmentId
            
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/update-department", updateDepartmentEvent);

        // Assert
        response = await httpClient.GetAsync($"{RequestUri}/{department.DepartmentId}/organizations/{customerId}");
        department = await response.Content.ReadFromJsonAsync<Department>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updateDepartmentEvent.Name, department.Name);
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task DeleteDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department1",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
            
        var customerId = _factory.ORGANIZATION_ID;

        // Act
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        DeleteDepartmentEvent departmentEvent = new DeleteDepartmentEvent()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/delete-department", departmentEvent);

        // Assert
        response = await httpClient.GetAsync($"{RequestUri}/{department.DepartmentId}/organizations/{customerId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task AssignDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        CreateEmployeeEvent newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            CustomerId = _customerId
        };
        response = await httpClient.PostAsJsonAsync($"{EmployeeRequestUri}/create-employee", newEmployeeEvent);
        var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
        
        // Act
        AssignDepartmentEvent assignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/employee-assign-department", assignDepartmentEvent);

        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var user = await response.Content.ReadFromJsonAsync<User>();
        
        // Assert
        Assert.Equal(newDepartmentEvent.Name, user.DepartmentName);
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task UnnassignDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        CreateEmployeeEvent newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            CustomerId = _customerId
        };
        response = await httpClient.PostAsJsonAsync($"{EmployeeRequestUri}/create-employee", newEmployeeEvent);
        var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
        AssignDepartmentEvent assignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/employee-assign-department", assignDepartmentEvent);

        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var userWithAssignedDepartment = await response.Content.ReadFromJsonAsync<User>();

        // Act
        UnassignDepartmentEvent unassignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/unassign-department", unassignDepartmentEvent);
        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var userWithUnAssignedDepartment = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(newDepartmentEvent.Name, userWithAssignedDepartment.DepartmentName);
        Assert.Null(userWithUnAssignedDepartment.DepartmentName);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task AssignManagerToDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        NewUser newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            Role = PredefinedRole.Manager.ToString(),
            CallerId = Guid.Empty,
            EmployeeId = "123",
            SkipAddingUserToOkta = true
        };
        response = await httpClient.PostAsJsonAsync($"api/v1/organizations/{_customerId}/users", newEmployeeEvent);
        var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
        
        //Act
        AssignDepartmentEvent assignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/assign-department-manager", assignDepartmentEvent);

        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var user = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(department.DepartmentId, user.ManagerOf.FirstOrDefault().DepartmentId);
        Assert.Equal(department.Name, user.ManagerOf.FirstOrDefault().DepartmentName);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task UnassignManagerFromDepartment_ReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateDepartmentEvent newDepartmentEvent = new()
        {
            Name = "Test Department",
            Description = "Test Department",
            CustomerId = _customerId,
            CostCenterId = "",
            CallerId = Guid.Empty,
            ParentDepartmentId = null,
            ManagedBy = new List<Guid>()
        };
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-department", newDepartmentEvent);
        var department = await response.Content.ReadFromJsonAsync<Department>();
        NewUser newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            Role = PredefinedRole.Manager.ToString(),
            CallerId = Guid.Empty,
            EmployeeId = "123",
            SkipAddingUserToOkta = true
        };
        response = await httpClient.PostAsJsonAsync($"api/v1/organizations/{_customerId}/users", newEmployeeEvent);
        var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
        
        AssignDepartmentEvent assignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/assign-department-manager", assignDepartmentEvent);

        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var userAsManager = await response.Content.ReadFromJsonAsync<User>();
        
        // Act
        UnassignDepartmentEvent unassignDepartmentEvent = new()
        {
            CustomerId = _customerId,
            DepartmentId = department.DepartmentId,
            UserId = userDto.Id,
            CallerId = Guid.Empty
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/unnassign-department-manager", unassignDepartmentEvent);

        response = await httpClient.GetAsync($"{EmployeeRequestUri}/{userDto.Id}/organizations/{_customerId}");
        var userNotAsManager = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(department.DepartmentId, userAsManager.ManagerOf.FirstOrDefault().DepartmentId);
        Assert.Equal(department.Name, userAsManager.ManagerOf.FirstOrDefault().DepartmentName);
        Assert.Null(userNotAsManager.ManagerOf.FirstOrDefault());
    }
}