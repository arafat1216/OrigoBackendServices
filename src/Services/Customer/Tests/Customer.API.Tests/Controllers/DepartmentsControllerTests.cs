using Common.Extensions;
using Common.Interfaces;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using System.Net.Http.Json;

namespace Customer.API.IntegrationTests.Controllers;

public class DepartmentsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _organizationId;
    private readonly Guid _departmentId;
    private readonly Guid _subDepartmentId;
    private readonly Guid _userOne;
    private readonly Guid _userTwo;
    private readonly Guid _userThree;
    private readonly Guid _userFour;
    private readonly Guid _userFive;
    private readonly string _userFourEmail;



    private readonly CustomerWebApplicationFactory<Startup> _factory;


    public DepartmentsControllerTests(CustomerWebApplicationFactory<Startup> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateClient();
        _organizationId = factory.ORGANIZATION_ID;
        _departmentId = factory.HEAD_DEPARTMENT_ID;
        _subDepartmentId = factory.SUB_DEPARTMENT_ID;
        _userOne = factory.USER_ONE_ID;
        _userTwo = factory.USER_TWO_ID;
        _userThree = factory.USER_THREE_ID;
        _userFour = factory.USER_FOUR_ID;
        _userFive = factory.USER_FIVE_ID;
        _userFourEmail = factory.USER_FOUR_EMAIL;
        _factory = factory;
        _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
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
    public async Task GetPaginatedDepartments_NoIncludes_Async()
    {
        var rawResponse = await _httpClient.GetAsync($"/api/v1/organizations/{_organizationId}/departments/paginated?includeManagers=false");
        var jsonResponse = await rawResponse.Content.ReadFromJsonAsync<PagedModel<Department>>();

        Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.True(jsonResponse.Items.Any());

        Assert.All(jsonResponse.Items, department => Assert.Empty(department.ManagedBy));
    }

    [Fact]
    public async Task GetPaginatedDepartments_WithIncludes_Async()
    {
        var rawResponse = await _httpClient.GetAsync($"/api/v1/organizations/{_organizationId}/departments/paginated?includeManagers=true");
        var jsonResponse = await rawResponse.Content.ReadFromJsonAsync<PagedModel<Department>>();

        Assert.Equal(HttpStatusCode.OK, rawResponse.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.True(jsonResponse.Items.Any());

        Assert.Contains(jsonResponse.Items, department => department.ManagedBy is not null);
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
            CostCenterId = "CostCenter",
            ManagedBy = new List<Guid> { _userOne, _userTwo }
        };

        var userEmailOne = "kari@normann.no";
        var userEmailTwo = "atish@normann.no";

        var response = await _httpClient.PostAsJsonAsync(requestUri, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<Department>();
        Assert.NotNull(department);
        Assert.Equal(2, department?.ManagedBy.Count);
        Assert.Equal(_userOne, department?.ManagedBy[0].UserId);
        Assert.Equal(userEmailOne, department?.ManagedBy[0].UserName);
        Assert.Equal("Kari Normann", department?.ManagedBy[0].Name);
        Assert.Equal(_userTwo, department?.ManagedBy[1].UserId);
        Assert.Equal("Atish Kumar", department?.ManagedBy[1].Name);
        Assert.Equal(userEmailTwo, department?.ManagedBy[1].UserName);

        //check Permissions
        var requestPermissionAfterOne = $"/api/v1/organizations/users/{userEmailOne}/permissions";
        var responsePermissionAfterOne = await _httpClient.GetAsync(requestPermissionAfterOne);
        Assert.Equal(HttpStatusCode.OK, responsePermissionAfterOne?.StatusCode);
        var permissionOneRead = await responsePermissionAfterOne?.Content.ReadFromJsonAsync<List<UserPermissions>>();
        Assert.Equal(2, permissionOneRead[0].AccessList.Count);
        Assert.Collection(permissionOneRead[0].AccessList,
          item => Assert.Equal(_organizationId, item),
          item => Assert.Equal(department?.DepartmentId, item));

        var requestPermissionAfterTwo = $"/api/v1/organizations/users/{userEmailTwo}/permissions";
        var responsePermissionAfterTwo = await _httpClient.GetAsync(requestPermissionTwo);
        Assert.Equal(HttpStatusCode.OK, responsePermissionAfterTwo?.StatusCode);
        var permissionReadTwo = await responsePermissionAfterTwo?.Content.ReadFromJsonAsync<List<UserPermissions>>();
        Assert.Equal(2, permissionReadTwo[0].AccessList.Count);
        Assert.Collection(permissionReadTwo[0].AccessList,
          item => Assert.Equal(_organizationId, item),
          item => Assert.Equal(department?.DepartmentId, item));

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
    public async Task UpdateDepartmentPatch_UpdateDepartmentManager_RemoveOldDepartmentManager()
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
            Role = "Manager",
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
            ManagedBy = new List<Guid> { _userFour, _userThree }
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
        Assert.Equal(2, departmentRead?.ManagedBy.Count);
        Assert.Collection(departmentRead?.ManagedBy,
           item => Assert.Equal(_userOne, item.UserId),
           item => Assert.Equal(_userTwo, item.UserId));
    }

    [Fact]
    public async Task UpdateDepartmentPatch_DepartmentManagers_WithoutManagers_AddingOnlyNewManagers()
    {
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var uriCreate = $"/api/v1/organizations/{_organizationId}/departments";

        var requestCreate = new NewDepartment
        {
            ParentDepartmentId = _departmentId,
            Name = "Department one",
            Description = "Test",
            CostCenterId = "CostCenter"
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
            ManagedBy = new List<Guid> { _userThree, _userFour }
        };

        var response = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{departmentCreate?.DepartmentId}", department);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var departmentRead = await response.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(2, departmentRead?.ManagedBy.Count);
        Assert.Collection(departmentRead?.ManagedBy,
          item => Assert.Equal(_userThree, item.UserId),
          item => Assert.Equal(_userFour, item.UserId));
    }
    [Fact]
    public async Task UpdateDepartmentPatch_OnlyAddNewManagers()
    {
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var firstUpdatedepartment = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userThree }
        };

        var responseFirst = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", firstUpdatedepartment);

        Assert.Equal(HttpStatusCode.OK, responseFirst.StatusCode);
        var firstDepartmentRead = await responseFirst.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(1, firstDepartmentRead?.ManagedBy.Count);
        Assert.Collection(firstDepartmentRead?.ManagedBy,
          item => Assert.Equal(_userThree, item.UserId));

        var secondUpdatedepartment = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userThree, _userFour }
        };

        var responseSecond = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", secondUpdatedepartment);

        Assert.Equal(HttpStatusCode.OK, responseSecond.StatusCode);
        var secondDepartmentRead = await responseSecond.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(2, secondDepartmentRead?.ManagedBy.Count);
        Assert.Collection(secondDepartmentRead?.ManagedBy,
          item => Assert.Equal(_userThree, item.UserId),
          item => Assert.Equal(_userFour, item.UserId));

        var thirdUpdatedepartment = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userThree, _userFour, _userThree, _userFour }
        };

        var responseThird = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", thirdUpdatedepartment);

        Assert.Equal(HttpStatusCode.OK, responseThird.StatusCode);
        var ThirdDepartmentRead = await responseThird.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(2, ThirdDepartmentRead?.ManagedBy.Count);
        Assert.Collection(ThirdDepartmentRead?.ManagedBy,
          item => Assert.Equal(_userThree, item.UserId),
          item => Assert.Equal(_userFour, item.UserId));

        var fourthUpdatedepartment = new UpdateDepartment
        {
            DepartmentId = _departmentId,
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userFive }
        };

        var responseFourth = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", fourthUpdatedepartment);

        Assert.Equal(HttpStatusCode.OK, responseFourth.StatusCode);
        var fourthDepartmentRead = await responseFourth.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(1, fourthDepartmentRead?.ManagedBy.Count);
        Assert.Collection(fourthDepartmentRead?.ManagedBy,
          item => Assert.Equal(_userFive, item.UserId));
    }

    [Fact]
    public async Task CreateDepartment_NoNewManagers_ParentDepartmentsManagersShouldBeAssignedAndHaveAccsess()
    {
        //update managers to a departmert
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var updatedepartment = new UpdateDepartment
        {
            Name = "First Department",
            DepartmentId = _departmentId,
            CallerId = Guid.NewGuid(),
            ManagedBy = new List<Guid> { _userFour }
        };
        var responseUpdatedepartment = await httpClient.PostAsJsonAsync(
        $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", updatedepartment);

        Assert.Equal(HttpStatusCode.OK, responseUpdatedepartment.StatusCode);
        var updatedepartmentRead = await responseUpdatedepartment.Content.ReadFromJsonAsync<Department>();
        Assert.Equal(1, updatedepartmentRead?.ManagedBy.Count);
        Assert.Collection(updatedepartmentRead?.ManagedBy,
          item => Assert.Equal(_userFour, item.UserId));

        //post a new sub department
        var newDepartment = new NewDepartment
        {
            Name = "Second Department",
            CostCenterId = "CostCenter123",
            Description = "Description",
            CallerId = Guid.NewGuid(),
            ParentDepartmentId = _departmentId
        };

        var response = await httpClient.PostAsJsonAsync(
            $"/api/v1/organizations/{_organizationId}/departments", newDepartment);
        var createdDepartment = await response.Content.ReadFromJsonAsync<Department>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(_departmentId, createdDepartment?.ParentDepartmentId);
        Assert.Equal(1, createdDepartment?.ManagedBy.Count);
        Assert.Collection(createdDepartment?.ManagedBy,
          item => Assert.Equal(_userFour, item.UserId));

        //The department id of the sub department
        var subdepartmentsId = createdDepartment.DepartmentId;

        //check if managers of parent department has accsess
        var requestPermission = $"/api/v1/organizations/users/{_userFourEmail}/permissions";
        var responsePermission = await httpClient.GetAsync(requestPermission);
        Assert.Equal(HttpStatusCode.OK, responsePermission?.StatusCode);
        var permissionRead = await responsePermission?.Content.ReadFromJsonAsync<List<UserPermissions>>();
        Assert.Equal(4, permissionRead[0].AccessList.Count);
        Assert.Collection(permissionRead[0].AccessList,
          item => Assert.Equal(_organizationId, item),
          item => Assert.Equal(_subDepartmentId, item),
          item => Assert.Equal(_departmentId, item),
          item => Assert.Equal(subdepartmentsId, item));

    }
}