using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Customer.Backend;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class UsersBackofficeControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<UsersBackofficeController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<UsersBackofficeController> _factory;
        private readonly ITestOutputHelper _output;

        public UsersBackofficeControllerTests(OrigoGatewayWebApplicationFactory<UsersBackofficeController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }



        [Fact]
        public async Task UserQuickSearch_Test_For_PartnerAdmin()
        {
            /*
             * Arrange
             */
            var partnerId = Guid.NewGuid();
            var organizationId1 = Guid.NewGuid();
            var organizationId2 = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(PredefinedRole.PartnerAdmin)));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, userId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId1.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId2.ToString()));

            Mock<IUserPermissionService> userPermissionServiceMock = new();
            userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None))
                                     .Returns(Task.FromResult(permissionsIdentity));

            Mock<IUserServices> hardwareServiceOrderService = new();
            hardwareServiceOrderService.Setup(_ => _.UserAdvancedSearch(It.IsAny<UserSearchParameters>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                       .ReturnsAsync(new PagedModel<OrigoUser>());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(hardwareServiceOrderService.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            /*
             * Act
             */
            var response = await client.GetAsync($"origoapi/v1.0/backoffice/users/search/quicksearch?q=searchQuery");

            /*
             * Asserts
             */
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Ensure the partner-admin's access-list were added to the organization filter
            hardwareServiceOrderService.Verify(e => e.UserAdvancedSearch(It.Is<UserSearchParameters>(e => e.OrganizationIds != null && e.OrganizationIds.Any()), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));
        }


        [Fact]
        public async Task UserQuickSearch_Test_For_SystemAdmin()
        {
            /*
             * Arrange
             */
            var partnerId = Guid.NewGuid();
            var organizationId1 = Guid.NewGuid();
            var organizationId2 = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(PredefinedRole.SystemAdmin)));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, userId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId1.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId2.ToString()));

            Mock<IUserPermissionService> userPermissionServiceMock = new();
            userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None))
                                     .Returns(Task.FromResult(permissionsIdentity));

            Mock<IUserServices> hardwareServiceOrderService = new();
            hardwareServiceOrderService.Setup(_ => _.UserAdvancedSearch(It.IsAny<UserSearchParameters>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                       .ReturnsAsync(new PagedModel<OrigoUser>());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(hardwareServiceOrderService.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            /*
             * Act
             */
            var response = await client.GetAsync($"origoapi/v1.0/backoffice/users/search/quicksearch?q=searchQuery");

            /*
             * Asserts
             */
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Ensure the system-admin don't get a organization-filter applied
            hardwareServiceOrderService.Verify(e => e.UserAdvancedSearch(It.Is<UserSearchParameters>(e => e.OrganizationIds == null), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));
        }


    }
}
