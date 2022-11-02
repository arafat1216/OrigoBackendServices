using Common.Extensions;
using Common.Interfaces;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using System.Net.Http.Json;
using Customer.API.CustomerDatasyncModels;
using CustomerServices.ServiceModels;
using Customer.API.ViewModels;

namespace Customer.API.IntegrationTests.Controllers;

public class CustomerDatasyncUsersControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{

    private readonly HttpClient _httpClient;
    private readonly Guid _customerId;
    private const string RequestUri = "api/v1/customer-datasync/users";

    private readonly CustomerWebApplicationFactory<Startup> _factory;

    public CustomerDatasyncUsersControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
    {
        _httpClient = factory.CreateDefaultClient();
        _customerId = factory.ORGANIZATION_ID;
        _factory = factory;
        _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.PubsubUserId().ToString());
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task CreateEmployee_WithInvalidUserData_ShouldReturnBadRequest()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        CreateEmployeeEvent newEmployeeEvent = new();

        // Act
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newEmployeeEvent);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task CreateEmployee_WithValidUserData_FetchNewEmployee()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var newEmployeeEmail = "test.mctest@test.techstep.io";
        CreateEmployeeEvent newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = newEmployeeEmail,
            MobileNumber = "91234567",
            CustomerId = _customerId
        };
        
        var customerId = _factory.ORGANIZATION_ID;

        // Act
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newEmployeeEvent);

        // Assert

        // Fetch newly created employee
        var response = await httpClient.GetAsync($"{RequestUri}/organizations/{customerId}?q={newEmployeeEmail}");
        var read = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newEmployeeEmail, read!.Items[0].Email);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task GetUserByPhoneNumber_WithValidPhoneNumber_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateEmployeeEvent newUser = await CreateUser(httpClient);
        var customerId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newUser);

        // Act
        var response = await httpClient.GetAsync($"{RequestUri}/{newUser.MobileNumber}/organizations/{customerId}");
        var read = await response.Content.ReadFromJsonAsync<UserDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newUser.Email, read.Email);
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task GetUserByPhoneNumber_WithInvalidPhoneNumber_ShouldReturn_NotFound()
    {
        
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateEmployeeEvent newUser = await CreateUser(httpClient);
        var INVALID_PHONE_NUMBER = "00000000";
        var customerId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newUser);

        // Act
        var response = await httpClient.GetAsync($"{RequestUri}/{INVALID_PHONE_NUMBER}/organizations/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task UpdateUser_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateEmployeeEvent newUser = await CreateUser(httpClient);
        var customerId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newUser);

        // Act
        var response = await httpClient.GetAsync($"{RequestUri}/{newUser.MobileNumber}/organizations/{customerId}");
        var user = await response.Content.ReadFromJsonAsync<User>();
        UpdateUserEvent updateUserEvent = new()
        {
            FirstName = "User Updated",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            CustomerId = _customerId,
            CallerId = Guid.Empty,
            EmployeeId = "123",
            UserId = user.Id
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/update-employee", updateUserEvent);
        response = await httpClient.GetAsync($"{RequestUri}/{newUser.MobileNumber}/organizations/{customerId}");
        var updatedUser = await response.Content.ReadFromJsonAsync<UserDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEqual(newUser.FirstName, updatedUser.FirstName);
        Assert.Equal(updateUserEvent.FirstName, updatedUser.FirstName);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task DeleteUser_ByUserId_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateEmployeeEvent newUser = await CreateUser(httpClient);
        var customerId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newUser);

        // Act
        var response = await httpClient.GetAsync($"{RequestUri}/{newUser.MobileNumber}/organizations/{customerId}");
        var user = await response.Content.ReadFromJsonAsync<User>();
        DeleteUserEvent deleteUserEvent = new()
        {
            CustomerId = _customerId,
            CallerId = Guid.Empty,
            UserId = user.Id,
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/delete-employee", deleteUserEvent);
        response = await httpClient.GetAsync($"{RequestUri}/{user.MobileNumber}/organizations/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task DeleteUser_ByPhoneNumber_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        CreateEmployeeEvent newUser = await CreateUser(httpClient);
        var customerId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newUser);

        // Act
        var response = await httpClient.GetAsync($"{RequestUri}/{newUser.MobileNumber}/organizations/{customerId}");
        var user = await response.Content.ReadFromJsonAsync<User>();
        DeleteUserByPhoneNumberEvent deleteUserEvent = new()
        {
            CustomerId = _customerId,
            CallerId = Guid.Empty,
            PhoneNumber = user.MobileNumber,
        };
        await httpClient.PostAsJsonAsync($"{RequestUri}/delete-employee-by-phone-number", deleteUserEvent);
        response = await httpClient.GetAsync($"{RequestUri}/{user.MobileNumber}/organizations/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<CreateEmployeeEvent> CreateUser(HttpClient httpClient)
    {
        CreateEmployeeEvent newEmployeeEvent = new()
        {
            FirstName = "Test",
            LastName = "Mctest",
            Email = "test.mctest@test.techstep.io",
            MobileNumber = "91234567",
            CustomerId = _customerId
        };
        
        await httpClient.PostAsJsonAsync($"{RequestUri}/create-employee", newEmployeeEvent);
        return newEmployeeEvent;
    }
}

