using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using Xunit;
using Xunit.Abstractions;

namespace Customer.API.IntegrationTests.Controllers;

public class DepartmentsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _organizationId;
    private readonly Guid _departmentId;
    private readonly Guid _userOne;
    private readonly Guid _userTwo;


    private readonly CustomerWebApplicationFactory<Startup> _factory;


    public DepartmentsControllerTests(CustomerWebApplicationFactory<Startup> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateClient();
        _organizationId = factory.ORGANIZATION_ID;
        _departmentId = factory.HEAD_DEPARTMENT_ID;
        _userOne = factory.USER_ONE_ID;
        _userTwo = factory.USER_TWO_ID;
        _factory = factory;
    }

    [Fact]
    public async Task GetDepartments()
    {
        var response = await _httpClient.GetAsync($"/api/v1/organizations/{_organizationId}/departments");

        var read = await response.Content.ReadFromJsonAsync<List<Department>>();

        _testOutputHelper.WriteLine(read?[0].Name);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateDepartment()
    {
        var newDepartment = new NewDepartment
        {
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
        };
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments", newDepartment);
        var createdDepartment = await response.Content.ReadFromJsonAsync<Department>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(newDepartment.Name, createdDepartment?.Name);
        Assert.Equal(newDepartment.CostCenterId, createdDepartment?.CostCenterId);
        Assert.Equal(newDepartment.Description, createdDepartment?.Description);
        Assert.Null(createdDepartment?.ParentDepartmentId);
        Assert.NotNull(createdDepartment?.DepartmentId);
    }

    [Fact]
    public async Task UpdateDepartmentPut()
    {
        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PutAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDepartmentPatch()
    {
        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDepartment()
    {
        var newDepartment = new NewDepartment
        {
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ParentDepartmentId = _organizationId
        };

        var createResponse = await _httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments", newDepartment);
        var createdDepartment = await createResponse.Content.ReadFromJsonAsync<Department>();

        var getResponse = await _httpClient.GetAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{createdDepartment?.DepartmentId.ToString()}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteDepartment()
    {
        var requestUri = $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}";

        var request = new HttpRequestMessage
        {
            Content = JsonContent.Create(Guid.NewGuid()),
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestUri, UriKind.Relative)
        };

        var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseGet = await _httpClient.GetAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}");

        Assert.Equal(HttpStatusCode.NotFound, responseGet.StatusCode);
    }
    [Fact]
    public async Task CreateDepartment_AddManagers()
    {
        //Get user
        var requestUserOne = $"/api/v1/organizations/users/{_userOne}";
        var responseUserOne = await _httpClient.GetAsync(requestUserOne);
        var requestUserTwo = $"/api/v1/organizations/users/{_userTwo}";
        var responseUserTwo = await _httpClient.GetAsync(requestUserTwo);

        var userOne = await responseUserOne.Content.ReadFromJsonAsync<User>();
        var userTwo = await responseUserTwo.Content.ReadFromJsonAsync<User>();

        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userOne);
        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userTwo);

        //Assign role to user
        var role = new NewUserPermission
        {
            AccessList = new List<Guid> { _organizationId },
            Role = "DepartmentManager",
            CallerId = Guid.NewGuid()
        };

        var requestPermissionsOne = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
        var putPermissionOne = _httpClient.PutAsJsonAsync(requestPermissionsOne, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionOne?.Result.StatusCode);

        var requestPermissionTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
        var putPermissionTwo = _httpClient.PutAsJsonAsync(requestPermissionTwo, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionTwo?.Result.StatusCode);
        var requestUri = $"/api/v1/organizations/{_organizationId}/departments";

        var request = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
           Name = "Department one",
           Description = "Test",
           CostCenterId= "CostCenter",
            ManagedBy = new List<Guid> {_userOne, _userTwo}
        };

        var response = await _httpClient.PostAsJsonAsync(requestUri, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(department);
        Assert.Equal(2, department?.ManagedBy.Count);
        Assert.Equal(_userOne, department?.ManagedBy[0].UserId);
        Assert.Equal("kari@normann.no", department?.ManagedBy[0].UserName);
        Assert.Equal("Kari Normann", department?.ManagedBy[0].Name);
        Assert.Equal(_userTwo, department?.ManagedBy[1].UserId);
        Assert.Equal("Atish Kumar", department?.ManagedBy[1].Name);
        Assert.Equal("atish@normann.no", department?.ManagedBy[1].UserName);

    }
    [Fact]
    public async Task CreateDepartment_AddManagers_NotRoleNotAssign()
    {
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var requestUri = $"/api/v1/organizations/{_organizationId}/departments";

        var request = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var response = await httpClient.PostAsJsonAsync(requestUri, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(department);
        Assert.Equal(0, department?.ManagedBy.Count);
    }

    [Fact]
    public async Task UpdateDepartmentPut_DepartmentManagers_UsersDontHaveManagerRole_ReturnsOk()
    {
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var UriCreate = $"/api/v1/organizations/{_organizationId}/departments";

        var requestCreate = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var responseCreate = await httpClient.PostAsJsonAsync(UriCreate, requestCreate);

        Assert.Equal(HttpStatusCode.Created, responseCreate.StatusCode);
        var departmentCreate = await responseCreate.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(departmentCreate);
        Assert.NotNull(departmentCreate?.DepartmentId);

        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var response = await httpClient.PutAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{departmentCreate?.DepartmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var departmentRead = await response.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(0, departmentRead?.ManagedBy.Count);

    }

    [Fact]
    public async Task UpdateDepartmentPatch_DepartmentManagers_WithNoDepartmentManagerRole_ReturnsOkAndNotAssignManagers()
    {
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var uriCreate = $"/api/v1/organizations/{_organizationId}/departments";

        var requestCreate = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var responseCreate = await httpClient.PostAsJsonAsync(uriCreate, requestCreate);

        Assert.Equal(HttpStatusCode.Created, responseCreate.StatusCode);
        var departmentCreate = await responseCreate.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(departmentCreate);
        Assert.NotNull(departmentCreate?.DepartmentId);

        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var response = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{departmentCreate?.DepartmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var departmentRead = await response.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(0, departmentRead?.ManagedBy.Count);
    }

    [Fact]
    public async Task UpdateDepartmentPatch_DepartmentManagers_UserWithRole_ReturnsOK()
    {
        //Get user
        var requestUserOne = $"/api/v1/organizations/users/{_userOne}";
        var responseUserOne = await _httpClient.GetAsync(requestUserOne);
        var requestUserTwo = $"/api/v1/organizations/users/{_userTwo}";
        var responseUserTwo = await _httpClient.GetAsync(requestUserTwo);

        var userOne = await responseUserOne.Content.ReadFromJsonAsync<User>();
        var userTwo = await responseUserTwo.Content.ReadFromJsonAsync<User>();

        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userOne);
        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userTwo);

        //Assign role to user
        var role = new NewUserPermission
        {
            AccessList = new List<Guid> { _organizationId },
            Role = "DepartmentManager",
            CallerId = Guid.NewGuid()
        };
        
        var requestPermissionsOne = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
        var putPermissionOne = _httpClient.PutAsJsonAsync(requestPermissionsOne, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionOne?.Result.StatusCode);

        var requestPermissionTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
        var putPermissionTwo = _httpClient.PutAsJsonAsync(requestPermissionTwo, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionTwo?.Result.StatusCode);

        var uriCreate = $"/api/v1/organizations/{_organizationId}/departments";

        var requestCreate = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var responseCreate = await _httpClient.PostAsJsonAsync(uriCreate, requestCreate);

        Assert.Equal(HttpStatusCode.Created, responseCreate.StatusCode);
        var departmentCreate = await responseCreate.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(departmentCreate);
        Assert.NotNull(departmentCreate?.DepartmentId);

        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{departmentCreate?.DepartmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var departmentRead = await response.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(4, departmentRead?.ManagedBy.Count);
    }
    [Fact]
    public async Task UpdateDepartmentPatch_DepartmentManagers_WithDepartmentManagerRoles_ReturnsOk()
    {
        //Get user
        var requestUserOne = $"/api/v1/organizations/users/{_userOne}";
        var responseUserOne = await _httpClient.GetAsync(requestUserOne);
        var requestUserTwo = $"/api/v1/organizations/users/{_userTwo}";
        var responseUserTwo = await _httpClient.GetAsync(requestUserTwo);

        var userOne = await responseUserOne.Content.ReadFromJsonAsync<User>();
        var userTwo = await responseUserTwo.Content.ReadFromJsonAsync<User>();

        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userOne);
        Assert.Equal(HttpStatusCode.OK, responseUserOne.StatusCode);
        Assert.NotNull(userTwo);

        //Assign role to user
        var role = new NewUserPermission
        {
            AccessList = new List<Guid> { _organizationId },
            Role = "DepartmentManager",
            CallerId = Guid.NewGuid()
        };

        var requestPermissionsOne = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
        var putPermissionOne = _httpClient.PutAsJsonAsync(requestPermissionsOne, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionOne?.Result.StatusCode);

        var requestPermissionTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
        var putPermissionTwo = _httpClient.PutAsJsonAsync(requestPermissionTwo, role);
        Assert.Equal(HttpStatusCode.OK, putPermissionTwo?.Result.StatusCode);

        var uriCreate = $"/api/v1/organizations/{_organizationId}/departments";

        var requestCreate = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var responseCreate = await _httpClient.PostAsJsonAsync(uriCreate, requestCreate);

        Assert.Equal(HttpStatusCode.Created, responseCreate.StatusCode);
        var departmentCreate = await responseCreate.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(departmentCreate);
        Assert.NotNull(departmentCreate?.DepartmentId);

        var department = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            Name = "Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{departmentCreate?.DepartmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var departmentRead = await response.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(4, departmentRead?.ManagedBy.Count);
    }
}