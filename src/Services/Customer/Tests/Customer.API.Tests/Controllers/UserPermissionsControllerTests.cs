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
using System.Threading.Tasks;
using Common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Customer.API.IntegrationTests.Controllers
{
    public class UserPermissionsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;
        private readonly Guid _userOneId;
        private readonly Guid _userTwoId;
        private readonly Guid _userThreeId;
        private readonly Guid _callerId;
        private readonly Guid _headDepartmentId;
        private readonly Guid _subDepartmentId;
        private readonly string _userOneEmail;
        private readonly string _userFourEmail;



        private readonly CustomerWebApplicationFactory<Startup> _factory;

        public UserPermissionsControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.ORGANIZATION_ID;
            _userOneId = factory.USER_ONE_ID;
            _userTwoId = factory.USER_TWO_ID;
            _userThreeId = factory.USER_THREE_ID;
            _userOneEmail = factory.USER_ONE_EMAIL;
            _headDepartmentId = factory.HEAD_DEPARTMENT_ID;
            _subDepartmentId = factory.SUB_DEPARTMENT_ID;
            _userFourEmail = factory.USER_FOUR_EMAIL;

            _callerId = Guid.NewGuid();
            _factory = factory;
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
        }
        [Fact]
        public async Task GetUserPermission()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);


            var requestUri = $"api/v1/organizations/users/{_userOneEmail}/permissions";

            var response = await httpClient.GetAsync(requestUri);
            var read = await response.Content.ReadFromJsonAsync<List<UserPermissions>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, read?.Count);
            Assert.Equal(1, read?[0].AccessList.Count);
            Assert.Equal("EndUser", read?[0].Role);
        }
        [Fact]
        public async Task AssignUsersPermissions()
        {
            var requestBody = new NewUsersPermission
            {
                UserPermissions = new List<NewUserPermissionDTO> 
                { 
                    new NewUserPermissionDTO 
                        { 
                            UserId = _userOneId,
                            AccessList = new List<Guid> { _customerId },
                            Role = "EndUser"
                        },
                    new NewUserPermissionDTO 
                        { 
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId, _subDepartmentId },
                            Role = "Manager" 
                    }
                }
            };
            var requestUri = $"/api/v1/organizations/users/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            var read = await response.Content.ReadFromJsonAsync<ViewModels.UsersPermissions>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, read?.UserPermissions.Count);

            Assert.Equal("EndUser",read?.UserPermissions[0].Role);
            Assert.Equal("Manager", read?.UserPermissions[1].Role);

            Assert.Equal(_userOneId, read?.UserPermissions[0].UserId);
            Assert.Equal(_userTwoId, read?.UserPermissions[1].UserId);

            Assert.Equal(1, read?.UserPermissions[0].AccessList.Count);
            Assert.Equal(2, read?.UserPermissions[1].AccessList.Count);

            var requestUriGet = $"api/v1/organizations/users/{_userOneEmail}/permissions";

            var responseGet = await _httpClient.GetAsync(requestUriGet);
            var readGet = await responseGet.Content.ReadFromJsonAsync<List<UserPermissions>>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, readGet?.Count);
        }
        [Fact]
        public async Task AssignUsersPermissions_CheckIfItHandlesErrors()
        {
            var requestBody = new NewUsersPermission
            {
                UserPermissions = new List<NewUserPermissionDTO>
                {
                    new NewUserPermissionDTO
                        {
                            UserId = _userOneId,
                            AccessList = new List<Guid> { _customerId },
                            Role = "EndUser"
                        },
                    new NewUserPermissionDTO
                        {
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId },
                            Role = "Headmaster"
                        },
                    new NewUserPermissionDTO
                        {
                            UserId = Guid.Empty,
                            AccessList = new List<Guid> { _customerId, _headDepartmentId },
                            Role = "DepartmentManager"
                        },
                    new NewUserPermissionDTO
                        {
                            UserId = _userThreeId,
                            AccessList = new List<Guid> { _customerId, _subDepartmentId },
                            Role = "DepartmentManager"
                    }
                }
            };
            var requestUri = $"/api/v1/organizations/users/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            var read = await response.Content.ReadFromJsonAsync<ViewModels.UsersPermissions>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(read);

            Assert.Equal(2, read?.UserPermissions.Count);
            Assert.Equal("EndUser", read?.UserPermissions[0].Role);
            Assert.Equal("DepartmentManager", read?.UserPermissions[1].Role);

            Assert.Equal(_userOneId, read?.UserPermissions[0].UserId);
            Assert.Equal(_userThreeId, read?.UserPermissions[1].UserId);

            Assert.Equal(1, read?.UserPermissions[0].AccessList.Count);
            Assert.Equal(2, read?.UserPermissions[1].AccessList.Count);

            Assert.Equal(2,read?.ErrorMessages.Count);
            Assert.Equal($"Invalid role name Headmaster for {_userTwoId}", read?.ErrorMessages[0]);
            Assert.Equal("User with id 00000000-0000-0000-0000-000000000000 does not exist", read?.ErrorMessages[1]);


        }
        [Fact]
        public async Task AssignUsersPermissions_NotFound_WhenListIsNull()
        {
            var requestBody = new NewUsersPermission
            {
                UserPermissions = new List<NewUserPermissionDTO>
                {
                    new NewUserPermissionDTO
                        {
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId },
                            Role = "Headmaster"
                        },
                    new NewUserPermissionDTO
                        {
                            UserId = Guid.Empty,
                            AccessList = new List<Guid> { _customerId, _headDepartmentId },
                            Role = "DepartmentManager"
                        }
                }
            };
            var requestUri = $"/api/v1/organizations/users/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AssignUsersPermissions_OnlyOne_PermissionForUser()
        {
            var requestBody = new NewUserPermission
            {
                Role = "DepartmentManager",
                AccessList = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000003") },
                CallerId = Guid.Empty
            };
            var requestUri = $"api/v1/organizations/users/{_userOneEmail}/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var requestUriGet = $"api/v1/organizations/users/{_userOneEmail}/permissions";

            var responseGet = await _httpClient.GetAsync(requestUriGet);
            var readGet = await responseGet.Content.ReadFromJsonAsync<List<UserPermissions>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, readGet?.Count);
            Assert.Equal(3, readGet?[0].AccessList.Count);
            Assert.Equal("DepartmentManager", readGet?[0].Role);

        }
        [Fact]
        public async Task AssignUsersPermissions_UpdatingPermissionWithSameRole()
        {
            var requestBody = new NewUserPermission
            {
                Role = "Manager",
                AccessList = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000001") },
                CallerId = Guid.Empty
            };
            
            var requestUri = $"api/v1/organizations/users/{_userFourEmail}/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var requestUriGet = $"api/v1/organizations/users/{_userFourEmail}/permissions";

            var responseGet = await _httpClient.GetAsync(requestUriGet);
            var readGet = await responseGet.Content.ReadFromJsonAsync<List<UserPermissions>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, readGet?.Count);
            Assert.Equal(2, readGet?[0].AccessList.Count);
            Assert.Equal("Manager", readGet?[0].Role);

        }
        [Fact]
        public async Task AssignUsersPermissions_BadRequest_TryToAddTwoDiffrentRolesForSameUser()
        {
            var requestBody = new NewUsersPermission
            {
                UserPermissions = new List<NewUserPermissionDTO>
                {
                    new NewUserPermissionDTO
                        {
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId },
                            Role = "EndUser"
                        },
                     new NewUserPermissionDTO
                        {
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId, _headDepartmentId },
                            Role = "DepartmentManager"
                        }
                }
            };
            var requestUri = $"/api/v1/organizations/users/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetUserPermission_ChangeUserStatusIfInvited()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Initiate onboarding process for customer - sends invitation for customers employees and changes user status to invited 
            var requestOnBoarding = $"/api/v1/organizations/{_customerId}/initiate-onboarding";
            var responseOnBoarding = await _httpClient.PostAsync(requestOnBoarding, null);
            var organization = await responseOnBoarding.Content.ReadFromJsonAsync<OrganizationDTO>();
            Assert.Equal(Common.Enums.CustomerStatus.StartedOnboardning, organization?.Status);
            Assert.Equal("StartedOnboardning", organization?.StatusName);

            //Get the permissions for user should also change status when status is invited
            var requestPermissions = $"api/v1/organizations/users/{_userOneEmail}/permissions";
            var responsePermissions = await httpClient.GetAsync(requestPermissions);
            var readPermissions = await responsePermissions.Content.ReadFromJsonAsync<List<UserPermissions>>();

            Assert.Equal(HttpStatusCode.OK, responsePermissions.StatusCode);
            Assert.Equal(1, readPermissions?.Count);

            var userId = readPermissions?[0].UserId;

            //Check user object if status is changed
            var requestUser = $"/api/v1/organizations/users/{userId}";
            var responseUser = await _httpClient.GetAsync(requestUser);

            var user = await responseUser.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, responseUser.StatusCode);
            Assert.NotNull(user);
            Assert.Equal("OnboardInitiated", user?.UserStatusName);
        }
    }
}
