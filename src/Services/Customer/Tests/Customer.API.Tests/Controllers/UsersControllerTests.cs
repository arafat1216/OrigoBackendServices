using Common.Enums;
using Common.Interfaces;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Customer.API.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;
        private readonly Guid _headDepartmentId;
        private readonly Guid _subDepartmentId;
        private readonly Guid _independentDepartmentId;
        private readonly Guid _userOneId;
        private readonly string _userOneEmail;
        private readonly Guid _userTwoId;
        private readonly Guid _userThreeId;
        private readonly Guid _callerId;
        private readonly Guid _userFourId;

        private readonly CustomerWebApplicationFactory<Startup> _factory;



        public UsersControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.ORGANIZATION_ID;
            _headDepartmentId = factory.HEAD_DEPARTMENT_ID;
            _subDepartmentId = factory.SUB_DEPARTMENT_ID;
            _independentDepartmentId = factory.INDEPENDENT_DEPARTMENT_ID;
            _userOneId = factory.USER_ONE_ID;
            _userTwoId = factory.USER_TWO_ID;
            _userThreeId = factory.USER_THREE_ID;
            _userOneEmail = factory.USER_ONE_EMAIL;
            _userFourId = factory.USER_FOUR_ID;
            _callerId = Guid.NewGuid();
            _factory = factory;
        }

        [Fact]
        public async Task GetAllUsers()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Setup
            var search = "";
            var page = 1;
            var limit = 1000;

            FilterOptionsForUser filterOptions = new FilterOptionsForUser {UserStatuses = new List<int>{1}};
            string json = JsonSerializer.Serialize(filterOptions);

            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}&filterOptions={json}";

            var response = await httpClient.GetAsync(requestUri);
            var users = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, users?.TotalItems);
        }

        [Fact]
        public async Task GetUserByEmail()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={_userOneEmail}";

            // Act
            var response = await httpClient.GetAsync(requestUri);
            var read = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, read?.TotalItems);
            Assert.Equal(_userOneEmail, read.Items[0].Email);
        }

        [Fact]
        public async Task GetAllUsers_DepartmentName()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var requestAssignDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}";
            var responseAssignDepartment = await httpClient.PostAsync(requestAssignDepartment, JsonContent.Create(_callerId));
            var content = responseAssignDepartment.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseAssignDepartment.StatusCode);
            Assert.Equal(_headDepartmentId, content?.Result?.AssignedToDepartment);
            Assert.Equal("Head department", content?.Result?.DepartmentName);


            // Setup
            var search = "";
            var page = 1;
            var limit = 1000;
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}&filterOptions={"{}"}";

            var response = await httpClient.GetAsync(requestUri);
            var read = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, read?.TotalItems);
            Assert.Equal("Head department", read?.Items[1].DepartmentName);
        }

        [Fact]
        public async Task UpdateUserPatch_ReturnsOK()
        {
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}";

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
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}";

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
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}";
            var response = await _httpClient.GetAsync(requestUri);

            var user = await response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(user);
            Assert.Equal("EndUser", user?.Role);
        }

        [Fact]
        public async Task GetUser_users()
        {
            var requestUri = $"/api/v1/organizations/users/{_userOneId}";
            var response = await _httpClient.GetAsync(requestUri);

            var user = await response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(user);
            Assert.Equal("ORGANIZATION ONE", user?.OrganizationName);
        }

        [Fact]
        public async Task AssignDepartment_CheckAsssignToDepartentList()
        {

            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}";
            var response = await _httpClient.PostAsync(requestUri, JsonContent.Create(_callerId));
            var read = response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(read);
            Assert.Equal(_headDepartmentId, read.Result?.AssignedToDepartment);
            Assert.Equal("Head department", read.Result?.DepartmentName);
        }

        [Fact]
        public async Task DeleteUser()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Delete User
            var url = $"/api/v1/organizations/{_customerId}/users/{_userTwoId}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Content = JsonContent.Create(_callerId);
            var deleteResponse = await httpClient.SendAsync(request);
            var deletedUser = await deleteResponse.Content.ReadFromJsonAsync<User>();
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.NotNull(deletedUser);
            Assert.Equal(_userTwoId, deletedUser!.Id);

            // Get Users
            var requestUri = $"/api/v1/organizations/{_customerId}/users?filterOptions={"{}"}";

            var getResponse = await httpClient.GetAsync(requestUri);
            var read = await getResponse.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal(3, read?.TotalItems);
        }

        [Fact]
        public async Task AssignManagerToDepartment_ShouldReturnOk()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get user
            var requestUriGet = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet = await httpClient.GetAsync(requestUriGet);

            var user = await responseGet.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(user);

            //Assign role to user
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "DepartmentManager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign user as department manager
            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response = await httpClient.PostAsync(requestUri, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Check permission
            var requestPermissionsCheck = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissionsCheck = await _httpClient.GetAsync(requestPermissionsCheck);

            var permission = await responsePermissionsCheck.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissionsCheck.StatusCode);
            Assert.Equal(1,permission?.Count);
            Assert.Equal(3,permission?[0].AccessList?.Count);
        }
        [Fact]
        public async Task UnassignManagerFromDepartment_ReturnsOk()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get user
            var requestUriGet = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet = await httpClient.GetAsync(requestUriGet);

            var user = await responseGet.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(user);

            //Assign role to user
            var role = new NewUserPermission 
            { 
                AccessList = new List<Guid> { _customerId }, 
                Role = "DepartmentManager", 
                CallerId = _callerId 
            };
            var requestPermissions = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign department manager to department

            var requestUri = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response = await httpClient.PostAsync(requestUri, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Get accsesslist
            var responsePermissions = await httpClient.GetAsync(requestPermissions);

            var permission = await responsePermissions.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions.StatusCode);
            Assert.Equal(1, permission?.Count);
            Assert.Equal("DepartmentManager", permission?[0].Role);
            Assert.Equal(3, permission?[0].AccessList.Count);


            //Remove department manager
            HttpRequestMessage requestUnassignManager = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var responseUnassignManager = await httpClient.SendAsync(requestUnassignManager);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager.StatusCode);

            var requestPermissionsAfter = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissionsAfter = await httpClient.GetAsync(requestPermissionsAfter);

            var permissionAfter = await responsePermissionsAfter.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissionsAfter.StatusCode);
            Assert.Equal(1, permissionAfter?.Count);
            Assert.Equal(1, permissionAfter?[0].AccessList?.Count);
            Assert.Equal("EndUser", permissionAfter?[0].Role);
        }
        [Fact]
        public async Task AssignManagerToDepartment_SubDepartmentsHaveDepartmentManager_ShouldHaveSubDepartmentsInAccsessList()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get users
            var requestUri_UserOne = $"/api/v1/organizations/users/{_userOneId}";
            var response_UserOne = await httpClient.GetAsync(requestUri_UserOne);
            var requestUriGet_UserTwo = $"/api/v1/organizations/users/{_userTwoId}";
            var responseGet_UserTwo = await httpClient.GetAsync(requestUriGet_UserTwo);

            var userOne = await response_UserOne.Content.ReadFromJsonAsync<User>();
            var userTwo = await responseGet_UserTwo.Content.ReadFromJsonAsync<User>();


            Assert.Equal(HttpStatusCode.OK, response_UserOne.StatusCode);
            Assert.NotNull(userOne);
            Assert.Equal(HttpStatusCode.OK, responseGet_UserTwo.StatusCode);
            Assert.NotNull(userTwo);

            //Assign role to user one
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "DepartmentManager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign roles to user two
            var roleTwo = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "DepartmentManager",
                CallerId = _callerId
            };
            var requestPermissionsTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
            var putPermissionTwo = httpClient.PutAsJsonAsync(requestPermissionsTwo, roleTwo);
            Assert.Equal(HttpStatusCode.OK, putPermissionTwo?.Result.StatusCode);

            //Assign manager to sub department
            var requestUri_subDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_subDepartmentId}/manager";
            var response_subDepartment = await httpClient.PostAsync(requestUri_subDepartment, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response_subDepartment.StatusCode);

            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userTwoId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response_headDepartment.StatusCode);

            var requestPermissions_userOne = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
            var responsePermissions_userOne = await httpClient.GetAsync(requestPermissions_userOne);

            var permissions_userOne = await responsePermissions_userOne.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_userOne.StatusCode);
            Assert.Equal(1, permissions_userOne?.Count);
            Assert.Equal("DepartmentManager", permissions_userOne?[0].Role);
            Assert.Equal(2, permissions_userOne?[0].AccessList.Count);


            var requestPermissions_userTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
            var responsePermissions_userTwo = await httpClient.GetAsync(requestPermissions_userTwo);

            var permissions_userTwo = await responsePermissions_userTwo.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_userTwo.StatusCode);
            Assert.Equal(1, permissions_userTwo?.Count);
            Assert.Equal("DepartmentManager", permissions_userTwo?[0].Role);
            Assert.Equal(3, permissions_userTwo?[0].AccessList?.Count);
        }
        [Fact]
        public async Task AssignManagerToDepartment_SubDepartmentsHaveDepartmentManager_ShouldRemoveDepartmentManagerPermission()
        {
            //Get users
            var requestUri_UserOne = $"/api/v1/organizations/users/{_userOneId}";
            var response_UserOne = await _httpClient.GetAsync(requestUri_UserOne);
            var requestUriGet_UserTwo = $"/api/v1/organizations/users/{_userTwoId}";
            var responseGet_UserTwo = await _httpClient.GetAsync(requestUriGet_UserTwo);

            var userOne = await response_UserOne.Content.ReadFromJsonAsync<User>();
            var userTwo = await responseGet_UserTwo.Content.ReadFromJsonAsync<User>();


            Assert.Equal(HttpStatusCode.OK, response_UserOne.StatusCode);
            Assert.NotNull(userOne);
            Assert.Equal(HttpStatusCode.OK, responseGet_UserTwo.StatusCode);
            Assert.NotNull(userTwo);

            //Assign role to user
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "DepartmentManager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
            var putPermission = _httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign roles to users
            var roleTwo = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "DepartmentManager",
                CallerId = _callerId
            };
            var requestPermissionsTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
            var putPermissionTwo = _httpClient.PutAsJsonAsync(requestPermissionsTwo, roleTwo);
            Assert.Equal(HttpStatusCode.OK, putPermissionTwo?.Result.StatusCode);

            //Assign manager to sub department
            var requestUri_subDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_subDepartmentId}/manager";
            var response_subDepartment = await _httpClient.PostAsync(requestUri_subDepartment, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response_subDepartment.StatusCode);

            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userTwoId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await _httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response_headDepartment.StatusCode);

            //Unassign user one as department manager
            HttpRequestMessage requestUnassignManager_userOne = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri_subDepartment, UriKind.Relative)
            };

            var responseUnassignManager_userOne = await _httpClient.SendAsync(requestUnassignManager_userOne);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager_userOne.StatusCode);

            //Unassign user two as department manager
            HttpRequestMessage requestUnassignManager_userTwo = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri_headDepartment, UriKind.Relative)
            };

            var responseUnassignManager_userTwo = await _httpClient.SendAsync(requestUnassignManager_userTwo);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager_userTwo.StatusCode);

            //Find permissions set for user one
            var requestPermissions_userOne = $"/api/v1/organizations/users/{userOne?.Email}/permissions";
            var responsePermissions_userOne = await _httpClient.GetAsync(requestPermissions_userOne);

            var permissions_userOne = await responsePermissions_userOne.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_userOne.StatusCode);
            Assert.Equal(1, permissions_userOne?.Count);
            Assert.Equal("EndUser", permissions_userOne?[0].Role);
            Assert.Equal(1, permissions_userOne?[0].AccessList.Count);

            //Find permissions set for user two
            var requestPermissions_userTwo = $"/api/v1/organizations/users/{userTwo?.Email}/permissions";
            var responsePermissions_userTwo = await _httpClient.GetAsync(requestPermissions_userTwo);

            var permissions_userTwo = await responsePermissions_userTwo.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_userTwo.StatusCode);
            Assert.Equal(1, permissions_userTwo?.Count);
            Assert.Equal("EndUser", permissions_userTwo?[0].Role);
            Assert.Equal(1, permissions_userTwo?[0].AccessList?.Count);
        }
        [Fact]
        public async Task UnAssignManagerToDepartment_UnConnectedDepartment_RemoveOnlyOneDepartment()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get user
            var requestUriGet = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet = await httpClient.GetAsync(requestUriGet);

            var user = await responseGet.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(user);

            //Assign role to user
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "Manager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Make new subdepartment for sub department
            var newDepartment = new NewDepartment
            {
                Name = "Department",
                CostCenterId = "CostCenter123",
                Description = "Description",
                CallerId = Guid.NewGuid(),
                ParentDepartmentId = null
            };
            var response = await httpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_customerId}/departments", newDepartment);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdDepartment = await response.Content.ReadFromJsonAsync<Department>();
            Assert.Null(createdDepartment?.ParentDepartmentId);
            Assert.NotNull(createdDepartment?.DepartmentId);


            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, response_headDepartment.StatusCode);

            //Assign manager to single department
            var requestUri_singleDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{createdDepartment?.DepartmentId}/manager";
            var response_singleDepartment = await httpClient.PostAsync(requestUri_singleDepartment, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.OK, response_singleDepartment.StatusCode);

            //Unassign user as department manager for single department
            HttpRequestMessage requestUnassignManager_user = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri_singleDepartment, UriKind.Relative)
            };

            var responseUnassignManager_user = await httpClient.SendAsync(requestUnassignManager_user);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager_user.StatusCode);

            //Find permissions set for user
            var requestPermissions_user = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissions_user = await httpClient.GetAsync(requestPermissions_user);

            var permissions_user = await responsePermissions_user.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_user.StatusCode);
            Assert.Equal(1, permissions_user?.Count);
            Assert.Equal("Manager", permissions_user?[0].Role);
            Assert.Equal(3, permissions_user?[0].AccessList?.Count);

        }
        [Fact]
        public async Task AssignManagerToDepartment_AssigningDepartmentManagerWithNoDepartmentManagerRole_NotAllowed()
        {

            //Get user
            var requestUriGet_User = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet_User = await _httpClient.GetAsync(requestUriGet_User);

            var user = await responseGet_User.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet_User.StatusCode);
            Assert.NotNull(user);

            //Find permissions set for user 
            var requestPermissions_user = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissions_user = await _httpClient.GetAsync(requestPermissions_user);
            var permissions_user = await responsePermissions_user.Content.ReadFromJsonAsync<List<UserPermissions>>();

            Assert.Equal(HttpStatusCode.OK, responsePermissions_user.StatusCode);
            Assert.Equal(1, permissions_user?.Count);
            Assert.Equal("EndUser", permissions_user?[0].Role);

            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await _httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.BadRequest, response_headDepartment.StatusCode);

        }

        [Fact]
        public async Task GetUsers_DepartmentManagers_CheckViewModel()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Assign manager
            var requestUriDepartment = $"/api/v1/organizations/{_customerId}/users/{_userThreeId}/department/{_headDepartmentId}/manager";
            var responseDepartment = await httpClient.PostAsync(requestUriDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, responseDepartment.StatusCode);

            //Get all users
            var search = "";
            var page = 1;
            var limit = 1000;
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}&filterOptions={"{}"}";

            var response = await httpClient.GetAsync(requestUri);
            var read = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, read?.TotalItems);
            Assert.Equal(1, read?.Items[2]?.ManagerOf.Count);
            Assert.Equal(_headDepartmentId, read?.Items[2]?.ManagerOf[0].DepartmentId);
            Assert.Equal("Head department", read?.Items[2]?.ManagerOf[0].DepartmentName);

            //Get user
            var requestUriUser = $"/api/v1/organizations/users/{_userThreeId}";
            var responseUser = await httpClient.GetAsync(requestUriUser);

            var user = await responseUser.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseUser.StatusCode);
            Assert.NotNull(user);
            Assert.Equal(_headDepartmentId, user?.ManagerOf[0].DepartmentId);
            Assert.Equal("Head department", user?.ManagerOf[0].DepartmentName);

            //Get user for customer
            var requestUriUserCustomer = $"/api/v1/organizations/{_customerId}/users/{_userThreeId}";
            var responseUserGetCustomer = await httpClient.GetAsync(requestUriUserCustomer);

            var customerUser = await responseUserGetCustomer.Content.ReadFromJsonAsync<User>();
            Assert.Equal(HttpStatusCode.OK, responseUserGetCustomer.StatusCode);
            Assert.NotNull(customerUser);
            Assert.Equal(_headDepartmentId, customerUser?.ManagerOf[0].DepartmentId);
            Assert.Equal("Head department", customerUser?.ManagerOf[0].DepartmentName);
        }
        [Fact]
        public async Task GetUserInfoFromUserName_Ok()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);


            var request = $"/api/v1/organizations/{_userOneEmail}/users-info";
            var response = await httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


            var userInfo = await response.Content.ReadFromJsonAsync<ViewModels.UserInfo>();

            Assert.NotNull(userInfo);
            Assert.Equal(_customerId, userInfo?.OrganizationId);
            Assert.Equal(_userOneId, userInfo?.UserId);
            Assert.Equal(Guid.Empty,userInfo?.DepartmentId);
            Assert.Equal(_userOneEmail, userInfo?.UserName);

        }
        [Fact]
        public async Task GetUserFromUserId_Ok()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var request = $"/api/v1/organizations/{_userOneId}/users-info";
            var response = await httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


            var userInfo = await response.Content.ReadFromJsonAsync<ViewModels.UserInfo>();

            Assert.NotNull(userInfo);
            Assert.Equal(_customerId, userInfo?.OrganizationId);
            Assert.Equal(_userOneId, userInfo?.UserId);
            Assert.Equal(Guid.Empty, userInfo?.DepartmentId);
            Assert.Equal(_userOneEmail, userInfo?.UserName);
        }
        [Fact]
        public async Task GetUserFromUserId_NotFound()
        {
            var NOT_VALID_GUID = Guid.Parse("012439f0-9cbc-42f4-8936-c1854865c1a7");

            var request = $"/api/v1/organizations/{NOT_VALID_GUID}/users-info";
            var response = await _httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetUserInfoFromUserName_NotFound()
        {
            var NOT_VALID_USERNAME = "mail@mail.com";

            var request = $"/api/v1/organizations/{NOT_VALID_USERNAME}/users-info";
            var response = await _httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task UnAssignManagerToDepartment_UserShouldGet_EndUserRole()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get user
            var requestUriGet = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet = await httpClient.GetAsync(requestUriGet);

            var user = await responseGet.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(user);

            //Assign role to user
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "Manager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, response_headDepartment.StatusCode);

            HttpRequestMessage requestUnassignManager_user = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri_headDepartment, UriKind.Relative)
            };

            var responseUnassignManager_user = await httpClient.SendAsync(requestUnassignManager_user);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager_user.StatusCode);

            //Find permissions set for user
            var requestPermissions_user = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissions_user = await httpClient.GetAsync(requestPermissions_user);

            var permissions_user = await responsePermissions_user.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_user.StatusCode);
            Assert.Equal(1, permissions_user?.Count);
            Assert.Equal("EndUser", permissions_user?[0].Role);
            Assert.Equal(1, permissions_user?[0].AccessList?.Count);
        }
        [Fact]
        public async Task UnAssignManagerToDepartment_UserIsManagerForAnotherDepartment_ShouldKeepManagerRole()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Get user
            var requestUriGet = $"/api/v1/organizations/users/{_userOneId}";
            var responseGet = await httpClient.GetAsync(requestUriGet);

            var user = await responseGet.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
            Assert.NotNull(user);

            //Assign role to user
            var role = new NewUserPermission
            {
                AccessList = new List<Guid> { _customerId },
                Role = "Manager",
                CallerId = _callerId
            };
            var requestPermissions = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var putPermission = httpClient.PutAsJsonAsync(requestPermissions, role);
            Assert.Equal(HttpStatusCode.OK, putPermission?.Result.StatusCode);

            //Assign manager to head department
            var requestUri_headDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_headDepartmentId}/manager";
            var response_headDepartment = await httpClient.PostAsync(requestUri_headDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, response_headDepartment.StatusCode);

            //Assign manager to independent department that is connected to same organization
            var requestUri_OtherNotConnectedDepartment = $"/api/v1/organizations/{_customerId}/users/{_userOneId}/department/{_independentDepartmentId}/manager";
            var response_OtherNotConnectedDepartment = await httpClient.PostAsync(requestUri_OtherNotConnectedDepartment, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, response_OtherNotConnectedDepartment.StatusCode);

            HttpRequestMessage requestUnassignManager_user = new HttpRequestMessage
            {
                Content = JsonContent.Create(_callerId),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri_headDepartment, UriKind.Relative)
            };
            

            //Unassign a manager
            var responseUnassignManager_user = await httpClient.SendAsync(requestUnassignManager_user);
            Assert.Equal(HttpStatusCode.OK, responseUnassignManager_user.StatusCode);

            //Find permissions set for user
            var requestPermissions_user = $"/api/v1/organizations/users/{user?.Email}/permissions";
            var responsePermissions_user = await httpClient.GetAsync(requestPermissions_user);

            var permissions_user = await responsePermissions_user.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, responsePermissions_user.StatusCode);
            Assert.Equal(1, permissions_user?.Count);
            Assert.Equal("Manager", permissions_user?[0].Role);
            Assert.Equal(2, permissions_user?[0].AccessList?.Count);
        }
        [Fact]
        public async Task CreateUserForCustomer_NewUserWithNoRole_ShouldGetEndUser()
        {

            var body = new NewUser
            {
                CallerId = _callerId,
                Email = "test@mail.com",
                FirstName = "test",
                LastName = "user",
                EmployeeId = "123",
                MobileNumber = "+479898645",
                UserPreference = new UserPreference { Language = "en" }
            };
            var request = $"/api/v1/organizations/{_customerId}/users";
            var response = await _httpClient.PostAsJsonAsync(request, body);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<ViewModels.User>();

            Assert.NotNull(user);
            Assert.Equal("EndUser", user?.Role);


        }
        [Fact]
        public async Task CreateUserForCustomer_NotValidRole_ShouldNotThrowExceptionOnlyGiveEnUserRole()
        {

            var body = new NewUser
            {
                CallerId = _callerId,
                Email = "test@mail.com",
                FirstName = "test",
                LastName = "user",
                EmployeeId = "123",
                MobileNumber = "+479898645",
                UserPreference = new UserPreference { Language = "en" },
                Role = "Boss"
            };
            var request = $"/api/v1/organizations/{_customerId}/users";
            var response = await _httpClient.PostAsJsonAsync(request, body);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<ViewModels.User>();

            Assert.NotNull(user);
            Assert.Equal("EndUser", user?.Role);


        }
        [Fact]
        public async Task GetUsersCount_OnlyCountActivatedUsers()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var filter = new FilterOptionsForUser { Roles = new[] { "SystemAdmin" } };
            string json = JsonSerializer.Serialize(filter);

            var request = $"/api/v1/organizations/{_customerId}/users/count/?filterOptions={json}";
            var response = await httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var count = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(1, count);
        }
        [Fact]
        public async Task GetUsersCount_NotValidACustomer()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            
            var filter = new FilterOptionsForUser { Roles = new[] { "SystemAdmin" } };
            string json = JsonSerializer.Serialize(filter);

            var request = $"/api/v1/organizations/{Guid.NewGuid()}/users/count/?filterOptions={json}";
            var response = await httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var count = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetUsersCount_DepartmentFilter_OnlyCountActivatedUsers()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var url = $"/api/v1/organizations/{_customerId}/users/{_userFourId}/department/{_headDepartmentId}";
            var assigneUserToDepartment = await httpClient.PostAsync(url, JsonContent.Create(_callerId));
            Assert.Equal(HttpStatusCode.OK, assigneUserToDepartment.StatusCode);

            var filter = new FilterOptionsForUser { AssignedToDepartments = new[] { _headDepartmentId} };
            string json = JsonSerializer.Serialize(filter);

            var request = $"/api/v1/organizations/{_customerId}/users/count/?filterOptions={json}";
            var response = await httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var count = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(1, count);
        }
        [Fact]
        public async Task GetUsersCount_FilterNull_ReturnsZero()
        {
            var filter = new FilterOptionsForUser { };
            string json = JsonSerializer.Serialize(filter);

            var request = $"/api/v1/organizations/{_customerId}/users/count/?filterOptions={json}";
            var response = await _httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var count = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GetUsersCount_NoFilterAtAll_ReturnsZeroAndNotException()
        {
            var request = $"/api/v1/organizations/{_customerId}/users/count";
            var response = await _httpClient.GetAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var count = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(0, count);
        }

    }
}
