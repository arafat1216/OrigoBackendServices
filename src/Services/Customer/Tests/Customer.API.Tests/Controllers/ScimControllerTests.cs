using System.Net.Http.Json;
using Common.Extensions;
using Common.Interfaces;
using Customer.API.CustomerDatasyncModels;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;

namespace Customer.API.IntegrationTests.Controllers;

public class ScimControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
{
    private readonly HttpClient _httpClient;
    private readonly Guid _customerId;
    private const string RequestUri = "api/v1/scim/users";
    private readonly CustomerWebApplicationFactory<Startup> _factory;
    private readonly Guid _callerId;

    public ScimControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
    {
        _httpClient = factory.CreateDefaultClient();
        _customerId = factory.ORGANIZATION_ID;
        _factory = factory;
        _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.PubsubUserId().ToString());
        _callerId = Guid.NewGuid();
    }

    private NewUser GetNewUser()
    {
        return new NewUser
        {
            CallerId = _callerId,
            Email = "test@mail.com",
            FirstName = "test",
            LastName = "user",
            EmployeeId = "123",
            MobileNumber = "+479898645",
            UserPreference = new UserPreference { Language = "en" }
        };
    }

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task CreateUser_Test()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

        var newUser = new NewUser
        {
            CallerId = _callerId,
            Email = "test@mail.com",
            FirstName = "test",
            LastName = "user",
            EmployeeId = "123",
            MobileNumber = "+479898645",
            UserPreference = new UserPreference { Language = "en" }
        };
        
        var organizationId = _factory.ORGANIZATION_ID;

        // Act
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser);
        var user = await response.Content.ReadFromJsonAsync<User>();

        // Fetch newly created employee
        response = await httpClient.GetAsync($"{RequestUri}/{user.Id.ToString()}");
        var read = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newUser.Email, read!.Email);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task UpdateUser_Test()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        var newUser = GetNewUser();
        var organizationId = _factory.ORGANIZATION_ID;
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser);
        var user = await response.Content.ReadFromJsonAsync<User>();
        
        // Act
        var updateUser = new UpdateUser
        {
            CallerId = _callerId,
            Email = "updatedtest@mail.com",
            FirstName = "Updated test",
            LastName = "Updated user",
            EmployeeId = "456",
            MobileNumber = "+479898123",
            UserPreference = new UserPreference { Language = "en" },
        };
        await httpClient.PutAsJsonAsync($"{RequestUri}/{user.Id}/organizations/{organizationId}", updateUser);

        // Fetch newly created employee
        response = await httpClient.GetAsync($"{RequestUri}/{user.Id.ToString()}");
        var result = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updateUser.Email, result.Email);
        Assert.Equal(updateUser.FirstName, result.FirstName);
        Assert.Equal(updateUser.LastName, result.LastName);
        Assert.Equal(updateUser.EmployeeId, result.EmployeeId);
        Assert.Equal(updateUser.MobileNumber, result.MobileNumber);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task DeleteUser_Test()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        var newUser = GetNewUser();
        var organizationId = _factory.ORGANIZATION_ID;
        var response = await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser);
        var user = await response.Content.ReadFromJsonAsync<User>();

        // Act
        var requestDelete = new HttpRequestMessage(HttpMethod.Delete, $"{RequestUri}/{user.Id}");
        requestDelete.Content = JsonContent.Create(_callerId);
        await httpClient.SendAsync(requestDelete);

        // Fetch newly created employee
        response = await httpClient.GetAsync($"{RequestUri}/{user.Id.ToString()}");
        var result = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task GetAllScimUsersAsync_Test()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
        var newUser1 = new NewUser
        {
            CallerId = _callerId,
            Email = "test1@mail.com",
            FirstName = "test1",
            LastName = "user1",
            EmployeeId = "111",
            MobileNumber = "+479898641",
            UserPreference = new UserPreference { Language = "en" }
        };
        var newUser2 = new NewUser
        {
            CallerId = _callerId,
            Email = "test2@mail.com",
            FirstName = "test2",
            LastName = "user2",
            EmployeeId = "222",
            MobileNumber = "+479898642",
            UserPreference = new UserPreference { Language = "en" }
        };
        var newUser3 = new NewUser
        {
            CallerId = _callerId,
            Email = "test3@mail.com",
            FirstName = "test3",
            LastName = "user3",
            EmployeeId = "333",
            MobileNumber = "+479898643",
            UserPreference = new UserPreference { Language = "en" }
        };
        var organizationId = _factory.ORGANIZATION_ID;
        await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser1);
        await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser2);
        await httpClient.PostAsJsonAsync($"{RequestUri}/organizations/{organizationId}", newUser3);

        // Act
        var responseAllUsers = await httpClient.GetAsync($"{RequestUri}");
        var allUsers = await responseAllUsers.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

        var responseUser1 = await httpClient.GetAsync($"{RequestUri}?userName={newUser1.Email}&startIndex={0}&limit={25}");
        var user1 = await responseUser1.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

        var responseUser2 = await httpClient.GetAsync($"{RequestUri}?userName={newUser2.Email}&startIndex={0}&limit={25}");
        var user2 = await responseUser2.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

        var responseUser3 = await httpClient.GetAsync($"{RequestUri}?userName={newUser3.Email}&startIndex={0}&limit={25}");
        var user3 = await responseUser3.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseAllUsers.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseUser1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseUser2.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseUser3.StatusCode);
        
        Assert.Equal(newUser1.Email, user1.Items.FirstOrDefault().Email);
        Assert.Equal(1, user1.Items.Count);
        Assert.Equal(1, user1.TotalItems);
        
        Assert.Equal(newUser2.Email, user2.Items.FirstOrDefault().Email);
        Assert.Equal(1, user2.Items.Count);
        Assert.Equal(1, user2.TotalItems);

        Assert.Equal(newUser3.Email, user3.Items.FirstOrDefault().Email);
        Assert.Equal(1, user3.Items.Count);
        Assert.Equal(1, user3.TotalItems);
    }
}