using Common.Interfaces;
using Customer.API.Controllers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Customer.API.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomerWebApplicationFactory<UsersController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;
        private readonly Guid _departmentId;
        private readonly Guid _userId;
        private readonly Guid _callerId;




        public UsersControllerTests(CustomerWebApplicationFactory<UsersController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = Extention.ORGANIZATION_ID;
            _departmentId = Extention.HEAD_DEPARTMENT_ID;
            _userId = Extention.USER_ID;
            _callerId = Guid.NewGuid();
        }

        [Fact]
        public async Task GetAllUsers()
        {

            // Setup
            var search = "";
            var page = 1;
            var limit = 1000;
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}";

            var response = await _httpClient.GetAsync(requestUri);
            var read = response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task GetAllUsers_DepartmentName()
        {
            var requestAssignDepartment = $"/api/v1/organizations/{_customerId}/users/{_userId}/department/{_departmentId}";
            var responseAssignDepartment = await _httpClient.PostAsync(requestAssignDepartment, JsonContent.Create(_callerId));
            var content = responseAssignDepartment.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseAssignDepartment.StatusCode);
            Assert.Equal(_departmentId, content?.Result?.AssignedToDepartment);
            Assert.Equal("Head department", content?.Result?.DepartmentName);


            // Setup
            var search = "";
            var page = 1;
            var limit = 1000;
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}";

            var response = await _httpClient.GetAsync(requestUri);
            var read = response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, read?.Result?.Items.Count);
            Assert.Equal("Head department", read?.Result?.Items[0].DepartmentName);

        }

        [Fact]
        public async Task UpdateUserPatch_ReturnsOK()
        {
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userId}";

            var updateUser = new UpdateUser
            {
                Email = "test@test.no",
                FirstName = "Test",
                LastName = "Testi",
                EmployeeId = "EID123",
                MobileNumber = "+4745454848",
                UserPreference = new UserPreference
                {
                    Language = "no"
                },
                CallerId = _callerId
            };
            var response = await _httpClient.PostAsJsonAsync(requestUri, updateUser);

            var user = response.Content.ReadFromJsonAsync<User>();


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test", user?.Result?.FirstName);
        }
        [Fact]
        public async Task UpdateUserPutAsync_ReturnsOK()
        {
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userId}";

            var updateUser = new UpdateUser
            {
                Email = "test@test.no",
                FirstName = "Test",
                LastName = "Testi",
                EmployeeId = "EID123",
                MobileNumber = "+4745454848",
                UserPreference = new UserPreference
                {
                    Language = "no"
                },
                CallerId = _callerId
            };
            var response = await _httpClient.PutAsJsonAsync(requestUri, updateUser);

            var user = response.Content.ReadFromJsonAsync<User>();


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Test", user?.Result?.FirstName);
        }
        [Fact]
        public async Task GetUser_WithRoles() 
        {
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userId}";
            var response = await _httpClient.GetAsync(requestUri); 

            var user = await response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(user);
            Assert.Equal("EndUser",user?.Role);
        }
       

        [Fact]
        public async Task GetUser_users()
        {
            var requestUri = $"/api/v1/organizations/users/{_userId}";
            var response = await _httpClient.GetAsync(requestUri);

            var user = await response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(user);
            Assert.Equal("ORGANIZATION ONE", user?.OrganizationName);
        }

        [Fact]
        public async Task AssignDepartment_CheckAsssignToDepartentList()
        {
            
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userId}/department/{_departmentId}";
            var response = await _httpClient.PostAsync(requestUri, JsonContent.Create(_callerId));
            var read = response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(read);
            Assert.Equal(_departmentId,read.Result?.AssignedToDepartment);
            Assert.Equal("Head department",read.Result?.DepartmentName);
        }
    }
}
