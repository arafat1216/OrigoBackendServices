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
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class CustomersControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<CustomersController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<CustomersController> _factory;
        private readonly ITestOutputHelper _output;

        public CustomersControllerTests(OrigoGatewayWebApplicationFactory<CustomersController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }
        public static IEnumerable<object[]> EmailAccess =>
       new List<object[]>
       {
            new object[] { "manager@test.io", "Manager", HttpStatusCode.Forbidden },
            new object[] { "enduser@test.io", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "departmentmanager@test.io", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "partnerAdmin@test.io", "PartnerAdmin", HttpStatusCode.OK },
            new object[] { "admin@test.io", "Admin", HttpStatusCode.OK },
            new object[] { "customerAdmin@test.io", "CustomerAdmin", HttpStatusCode.OK },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK}
       };

        [Theory]
        [MemberData(nameof(EmailAccess))]
        public async Task Get_SecurePageAccessibleOnlyByAdminUsers(string email, string role, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();
                    var organization = new Organization { OrganizationId = organizationId };
                    customerService.Setup(_ => _.InitiateOnbardingAsync(organizationId)).ReturnsAsync(organization);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsync($"/origoapi/v1.0/customers/{organizationId}/initiate-onboarding", null);

            Assert.Equal(expected, response.StatusCode);
        }

        public static IEnumerable<object[]> EmailAccessUserCount => new List<object[]>
        {
            new object[] { "manager@test.io", "Manager", HttpStatusCode.Forbidden },
            new object[] { "enduser@test.io", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "departmentmanager@test.io", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "partnerAdmin@test.io", "PartnerAdmin", HttpStatusCode.OK },
            new object[] { "admin@test.io", "Admin", HttpStatusCode.OK },
            new object[] { "customerAdmin@test.io", "CustomerAdmin", HttpStatusCode.OK },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK}
        };
        [Theory]
        [MemberData(nameof(EmailAccessUserCount))]
        public async Task userCount_SecurePageAccessibleOnlyForGivenRoles(string email, string role, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();

                    var count = new List<CustomerUserCount>
                    {
                        new CustomerUserCount {OrganizationId = organizationId,Count = 1, NotOnboarded = 1}
                    };

                    customerService.Setup(_ => _.GetCustomerUsersAsync(It.IsAny<FilterOptionsForUser>())).ReturnsAsync(count);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/userCount");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrganization_WithoutPayrollContactEmail()
        {
            var organizationId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "CustomerAdmin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();

                    customerService.Setup(_ => _.UpdateOrganizationAsync(It.IsAny<UpdateOrganizationDTO>())).ReturnsAsync(new Organization());
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            var postData = new UpdateOrganization()
            {
                OrganizationId = organizationId,
                Name = "TestName",
                OrganizationNumber = "919724617",
                ContactPerson = new OrigoContactPerson()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test@techstep.no",
                    PhoneNumber = "+4790909090",
                },
                Address = new Address
                {
                    City = "OSLO",
                    Country = "NO",
                    Postcode = "0554",
                    Street = "Markveien 32F"
                },
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsJsonAsync($"/origoapi/v1.0/customers", postData);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}