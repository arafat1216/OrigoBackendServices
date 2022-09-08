
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class UserPermissionsControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<UserPermissionsController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<UserPermissionsController> _factory;
        private readonly ITestOutputHelper _output;

        public UserPermissionsControllerTests(OrigoGatewayWebApplicationFactory<UserPermissionsController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }
       
        [Fact]
        public async Task GetPermissions_ManagerShouldContainMultipleGuidsInAccessList()
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@mail.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

            
            var userInfo = new UserInfoDTO 
            { 
                OrganizationId = organizationId,
                UserName = "user@mail.com",
                DepartmentId = departmentId
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    var userPermissions = new List<OrigoUserPermissions> { new OrigoUserPermissions() };
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsAsync(userInfo.UserName)).ReturnsAsync(userPermissions);
                    
                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(userInfo.UserName, Guid.Empty)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            var response = await client.GetAsync($"origoapi/v1.0/customers/users/{userInfo.UserName}/permissions");

            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public static IEnumerable<object[]> TestAdminAssigningAllPermission => new List<object[]>
        {
            //Admin
            new object[] {  "Admin", "Admin", HttpStatusCode.Created },
            new object[] {  "Admin", "CustomerAdmin", HttpStatusCode.Created },
            new object[] {  "Admin", "Manager", HttpStatusCode.Created },
            new object[] {  "Admin", "DepartmentManager", HttpStatusCode.Created },
            new object[] {  "Admin", "EndUser", HttpStatusCode.Created },
            new object[] {  "Admin", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "SystemAdmin", HttpStatusCode.Forbidden }, 
            //Customer admin
            new object[] {  "CustomerAdmin", "SystemAdmin", HttpStatusCode.Forbidden }, 
            new object[] {  "CustomerAdmin", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden }, 
            new object[] {  "CustomerAdmin", "PartnerAdmin", HttpStatusCode.Forbidden }, 
            new object[] {  "CustomerAdmin", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] {  "CustomerAdmin", "EndUser", HttpStatusCode.Created },
            new object[] {  "CustomerAdmin", "DepartmentManager", HttpStatusCode.Created }, 
            new object[] {  "CustomerAdmin", "Manager", HttpStatusCode.Created }, 
            new object[] {  "CustomerAdmin", "CustomerAdmin", HttpStatusCode.Created }, 
            new object[] {  "CustomerAdmin", "Admin", HttpStatusCode.Created }
        };
        [Theory]
        [MemberData(nameof(TestAdminAssigningAllPermission))]
        public async Task AddUsersPermissions_AdminShouldOnlyBeAbleToAssignRolesFromAdminAndDown(string callerRole, string newRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "user@test.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var updatedUserPermission = new NewUsersPermissions
            { 
                UserPermissions = new List<NewUserPermission>
                {
                    new NewUserPermission
                    {
                        UserId =  userId,
                        Role = newRole,
                        AccessList = new List<Guid>{ organizationId }
                    }
                }
            };

            var userInfo = new UserInfoDTO { OrganizationId = organizationId, UserName = "test@test.com", UserId = userId };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    userPermissionServiceMock.Setup(_ => _.AddUsersPermissionsForUsersAsync(It.IsAny<NewUsersPermissionsDTO>())).ReturnsAsync(It.IsAny<OrigoUsersPermissions>());

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(null, userId)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsJsonAsync($"origoapi/v1.0/customers/users/permissions", updatedUserPermission);

            Assert.Equal(expected, response.StatusCode);
        }
        public static IEnumerable<object[]> TestManagerAssigningAllPermission => new List<object[]>
        {
            //Manager
            new object[] { "Manager", "Admin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "Manager", HttpStatusCode.Created },
            new object[] { "Manager", "DepartmentManager", HttpStatusCode.Created },
            new object[] { "Manager", "EndUser", HttpStatusCode.Created },
            new object[] { "Manager", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "SystemAdmin", HttpStatusCode.Forbidden }, 
            //Department Manager
            new object[] { "DepartmentManager", "SystemAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "EndUser", HttpStatusCode.Created },
            new object[] { "DepartmentManager", "DepartmentManager", HttpStatusCode.Created },
            new object[] { "DepartmentManager", "Manager", HttpStatusCode.Created },
            new object[] { "DepartmentManager", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "Admin", HttpStatusCode.Forbidden }
        };
        [Theory]
        [MemberData(nameof(TestManagerAssigningAllPermission))]
        public async Task AddUsersPermissions_SecureThatManagerOnlyAbleToAssignManagersAndDown(string callerRole, string newRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@mail.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

            var updatedUserPermission = new NewUsersPermissions
            {
                UserPermissions = new List<NewUserPermission>
                {
                    new NewUserPermission
                    {
                        UserId =  userId,
                        Role = newRole,
                        AccessList = new List<Guid>{ organizationId }
                    }
                }
            };

            
            var userInfo = new UserInfoDTO { OrganizationId = organizationId, UserName = "user@mail.com", UserId = userId, DepartmentId = departmentId };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    userPermissionServiceMock.Setup(_ => _.AddUsersPermissionsForUsersAsync(It.IsAny<NewUsersPermissionsDTO>())).ReturnsAsync(It.IsAny<OrigoUsersPermissions>());

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(null, userId)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsJsonAsync($"origoapi/v1.0/customers/users/permissions", updatedUserPermission);

            Assert.Equal(expected, response.StatusCode);
        }
        public static IEnumerable<object[]> ManagerNotAllowedToAssignPermissions => new List<object[]>
        {
            //Manager
            new object[] { "Manager", "Manager", HttpStatusCode.Forbidden },
            new object[] { "Manager", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "Manager", "EndUser", HttpStatusCode.Forbidden }, 
            //Department Manager
            new object[] { "DepartmentManager", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "Manager", HttpStatusCode.Forbidden }
        };
        [Theory]
        [MemberData(nameof(ManagerNotAllowedToAssignPermissions))]
        public async Task AddUsersPermissions_SecureManagerNotAbleToAssignPermissionsForUsersNotInOwnDepartment(string callerRole, string newRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var differentDepartmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@mail.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

            var updatedUserPermission = new NewUsersPermissions
            {
                UserPermissions = new List<NewUserPermission>
                {
                    new NewUserPermission
                    {
                        UserId =  userId,
                        Role = newRole,
                        AccessList = new List<Guid>{ organizationId }
                    }
                }
            };


            var userInfo = new UserInfoDTO 
            { 
                OrganizationId = organizationId, 
                UserName = "user@mail.com", 
                UserId = userId, 
                DepartmentId = differentDepartmentId 
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    userPermissionServiceMock.Setup(_ => _.AddUsersPermissionsForUsersAsync(It.IsAny<NewUsersPermissionsDTO>())).ReturnsAsync(It.IsAny<OrigoUsersPermissions>());

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(null, userId)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsJsonAsync($"origoapi/v1.0/customers/users/permissions", updatedUserPermission);

            Assert.Equal(expected, response.StatusCode);
        }

        public static IEnumerable<object[]> TestAdminRemoveAllPermission => new List<object[]>
        {
            //Admin
            new object[] {  "Admin", "Admin", HttpStatusCode.OK },
            new object[] {  "Admin", "CustomerAdmin", HttpStatusCode.OK },
            new object[] {  "Admin", "Manager", HttpStatusCode.OK },
            new object[] {  "Admin", "DepartmentManager", HttpStatusCode.OK },
            new object[] {  "Admin", "EndUser", HttpStatusCode.OK },
            new object[] {  "Admin", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] {  "Admin", "SystemAdmin", HttpStatusCode.Forbidden },
            //Customer admin
            new object[] {  "CustomerAdmin", "SystemAdmin", HttpStatusCode.Forbidden },
            new object[] {  "CustomerAdmin", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] {  "CustomerAdmin", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] {  "CustomerAdmin", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] {  "CustomerAdmin", "EndUser", HttpStatusCode.OK },
            new object[] {  "CustomerAdmin", "DepartmentManager", HttpStatusCode.OK },
            new object[] {  "CustomerAdmin", "Manager", HttpStatusCode.OK },
            new object[] {  "CustomerAdmin", "CustomerAdmin", HttpStatusCode.OK },
            new object[] {  "CustomerAdmin", "Admin", HttpStatusCode.OK }
        };
        [Theory]
        [MemberData(nameof(TestAdminRemoveAllPermission))]
        public async Task RemoveUserPermission_SecureAdmin(string callerRole, string removeRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@test.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanDeleteCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));


            
            var userInfo = new UserInfoDTO
            {
                OrganizationId = organizationId,
                UserName = "user@mail.com"
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    //New permission 
                    var newPermission = new OrigoUserPermissions
                    {
                        AccessList = new List<Guid> { organizationId },
                        Role = removeRole

                    };
                    userPermissionServiceMock.Setup(_ => _.RemoveUserPermissionsForUserAsync(userInfo.UserName,It.IsAny<NewUserPermissionsDTO>())).ReturnsAsync(newPermission);

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(userInfo.UserName, Guid.Empty)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            
            var sendUserPermission = new NewUserPermissions
            {
                AccessList = new List<Guid> { organizationId },
                Role = removeRole
            };

            var encodedEmail = WebUtility.UrlEncode(userInfo.UserName);
            var requestUri = $"origoapi/v1.0/customers/users/{encodedEmail}/permissions";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(sendUserPermission),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await client.SendAsync(request);

            Assert.Equal(expected, response.StatusCode);
        }
        public static IEnumerable<object[]> TestManagerRemoveAllPermission => new List<object[]>
        {
            //Manager
            new object[] { "Manager", "Admin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "Manager", HttpStatusCode.OK },
            new object[] { "Manager", "DepartmentManager", HttpStatusCode.OK },
            new object[] { "Manager", "EndUser", HttpStatusCode.OK },
            new object[] { "Manager", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] { "Manager", "SystemAdmin", HttpStatusCode.Forbidden }, 
            //Department Manager
            new object[] { "DepartmentManager", "SystemAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "PartnerAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "EndUser", HttpStatusCode.OK },
            new object[] { "DepartmentManager", "DepartmentManager", HttpStatusCode.OK },
            new object[] { "DepartmentManager", "Manager", HttpStatusCode.OK },
            new object[] { "DepartmentManager", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "Admin", HttpStatusCode.Forbidden }
        };
        [Theory]
        [MemberData(nameof(TestManagerRemoveAllPermission))]
        public async Task RemoveUserPermission_SecureManager(string callerRole, string removeRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@test.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanDeleteCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

            
            var userInfo = new UserInfoDTO
            {
                OrganizationId = organizationId,
                UserName = "user@mail.com",
                UserId = userId,
                DepartmentId = departmentId
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    //New permission 
                    var newPermission = new OrigoUserPermissions
                    {
                        AccessList = new List<Guid> { organizationId },
                        Role = removeRole

                    }; userPermissionServiceMock.Setup(_ => _.RemoveUserPermissionsForUserAsync(userInfo.UserName, It.IsAny<NewUserPermissionsDTO>())).ReturnsAsync(newPermission);

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(userInfo.UserName, Guid.Empty)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            var sendUserPermission = new NewUserPermissions
            {
                AccessList = new List<Guid> { organizationId },
                Role = removeRole
            };

            var encodedEmail = WebUtility.UrlEncode(userInfo.UserName);
            var requestUri = $"origoapi/v1.0/customers/users/{encodedEmail}/permissions";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(sendUserPermission),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await client.SendAsync(request);

            Assert.Equal(expected, response.StatusCode);
        }
        public static IEnumerable<object[]> ManagerNotAllowedToRemovePermissions => new List<object[]>
        {
            //Manager
            new object[] { "Manager", "Manager", HttpStatusCode.Forbidden },
            new object[] { "Manager", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "Manager", "EndUser", HttpStatusCode.Forbidden }, 
            //Department Manager
            new object[] { "DepartmentManager", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "DepartmentManager", "Manager", HttpStatusCode.Forbidden }
        };
        [Theory]
        [MemberData(nameof(ManagerNotAllowedToRemovePermissions))]
        public async Task RemoveUserPermission_SecureManagerCantRemovePermissionsWhenNotInDepartment(string callerRole, string removeRole, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var differentDepartmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "caller@test.com";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, callerRole));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanDeleteCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

            var userInfo = new UserInfoDTO
            {
                OrganizationId = organizationId,
                UserName = "user@test.com",
                UserId = userId,
                DepartmentId = differentDepartmentId
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    //New permission 
                    var newPermission = new OrigoUserPermissions
                    {
                        AccessList = new List<Guid> { organizationId },
                        Role = removeRole

                    }; userPermissionServiceMock.Setup(_ => _.RemoveUserPermissionsForUserAsync(email, It.IsAny<NewUserPermissionsDTO>())).ReturnsAsync(newPermission);

                    var userService = new Mock<IUserServices>();
                    userService.Setup(_ => _.GetUserInfo(email, Guid.Empty)).ReturnsAsync(userInfo);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(userService.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            var sendUserPermission = new NewUserPermissions
            {
                AccessList = new List<Guid> { organizationId },
                Role = removeRole
            };

            var encodedEmail = WebUtility.UrlEncode(email);
            var requestUri = $"origoapi/v1.0/customers/users/{encodedEmail}/permissions";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(sendUserPermission),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await client.SendAsync(request);

            Assert.Equal(expected, response.StatusCode);
        }

    }

}
