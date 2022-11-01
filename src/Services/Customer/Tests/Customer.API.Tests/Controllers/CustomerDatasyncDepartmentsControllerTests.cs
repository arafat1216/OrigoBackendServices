using System.Net.Http.Json;
using Common.Extensions;
using Common.Interfaces;
using Common.Model.EventModels.DatasyncModels;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using CustomerServices.ServiceModels;
using Customer.API.ViewModels;

namespace Customer.API.IntegrationTests.Controllers;

public class CustomerDatasyncDepartmentsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{
    private readonly HttpClient _httpClient;
    private readonly Guid _customerId;
    private const string RequestUri = "api/v1/customer-datasync/departments";

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
}