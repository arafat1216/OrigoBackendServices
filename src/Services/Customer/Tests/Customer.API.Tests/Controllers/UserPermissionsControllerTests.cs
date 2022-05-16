using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

            _callerId = Guid.NewGuid();
            _factory = factory;
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
                            AccessList = new List<Guid> { _customerId },
                            Role = "EndUser"
                        },
                    new NewUserPermissionDTO
                        {  
                            UserId = _userOneId,
                            AccessList = new List<Guid> { _customerId, _headDepartmentId },
                            Role = "DepartmentManager"
                        },
                    new NewUserPermissionDTO 
                        { 
                            UserId = _userTwoId,
                            AccessList = new List<Guid> { _customerId, _subDepartmentId },
                            Role = "DepartmentManager" 
                    }
                }
            };
            var requestUri = $"/api/v1/organizations/users/permissions";

            var response = await _httpClient.PutAsJsonAsync(requestUri, requestBody);
            var read = await response.Content.ReadFromJsonAsync<ViewModels.UsersPermissions>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, read?.UserPermissions.Count);

            Assert.Equal("EndUser",read?.UserPermissions[0].Role);
            Assert.Equal("EndUser", read?.UserPermissions[1].Role);
            Assert.Equal("DepartmentManager", read?.UserPermissions[2].Role);
            Assert.Equal("DepartmentManager", read?.UserPermissions[3].Role);

            Assert.Equal(_userOneId, read?.UserPermissions[0].UserId);
            Assert.Equal(_userTwoId, read?.UserPermissions[1].UserId);
            Assert.Equal(_userOneId, read?.UserPermissions[2].UserId);
            Assert.Equal(_userTwoId, read?.UserPermissions[3].UserId);

            Assert.Equal(1, read?.UserPermissions[0].AccessList.Count);
            Assert.Equal(1, read?.UserPermissions[1].AccessList.Count);
            Assert.Equal(2, read?.UserPermissions[2].AccessList.Count);
            Assert.Equal(2, read?.UserPermissions[3].AccessList.Count);
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
                            UserId = _userTwoId,
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
            Assert.Equal(_userTwoId, read?.UserPermissions[1].UserId);

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
    }
}
