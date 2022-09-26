using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class PartnersControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<PartnersController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<PartnersController> _factory;
        private readonly ITestOutputHelper _output;

        public PartnersControllerTests(OrigoGatewayWebApplicationFactory<PartnersController> factory, ITestOutputHelper output)
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
            new object[] { "admin@test.io", "Admin", HttpStatusCode.Forbidden },
            new object[] { "customerAdmin@test.io", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK}
       };

        [Theory]
        [MemberData(nameof(EmailAccess))]
        public async Task GetPartnersAsync_SecurePageAccessibleByUserWithPartnerRights(string email, string role, HttpStatusCode expected)
        {
            var partnerId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

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

                    var partnerServiceMock = new Mock<IPartnerServices>();
                    var partner = new Partner { Name = "Partner", Id = partnerId };
                    var partners = new List<Partner>() { partner };
                    partnerServiceMock.Setup(_ => _.GetPartnersAsync()).ReturnsAsync(partners);
                    partnerServiceMock.Setup(_ => _.GetPartnerAsync(partnerId)).ReturnsAsync(partner);
                    services.AddSingleton(partnerServiceMock.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/partners");

            Assert.Equal(expected, response.StatusCode);
        }

        
        [Theory]
        [MemberData(nameof(EmailAccess))]
        public async Task GetPartnerAsync_SecurePageAccessibleByUserWithPartnerRights(string email, string role, HttpStatusCode expected)
        {
            var partnerId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

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

                    var partnerServiceMock = new Mock<IPartnerServices>();
                    var partner = new Partner { Name = "Partner", Id = partnerId };
                    partnerServiceMock.Setup(_ => _.GetPartnerAsync(partnerId)).ReturnsAsync(partner);
                    services.AddSingleton(partnerServiceMock.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/partners/{partnerId}");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task GetPartnerAsync_PartnerWithNoAccess_Forbid()
        {
            var partnerId = Guid.NewGuid();
            var partnerNoRightsId = Guid.NewGuid();
            var email = "partner@admin.com";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

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
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/partners/{partnerNoRightsId}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
