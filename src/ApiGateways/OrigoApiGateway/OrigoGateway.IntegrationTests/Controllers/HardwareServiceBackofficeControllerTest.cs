using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class HardwareServiceBackofficeControllerTest : IClassFixture<OrigoGatewayWebApplicationFactory<HardwareServiceBackofficeController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<HardwareServiceBackofficeController> _factory;
        private readonly ITestOutputHelper _output;
        
        public HardwareServiceBackofficeControllerTest(OrigoGatewayWebApplicationFactory<HardwareServiceBackofficeController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }

        [Fact]
        public async Task GetServiceOrderByIdAndOrganizationAsync_Test_For_PartnerAdmin()
        {
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
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId1.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId2.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => 
                        _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None))
                        .Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                    
                    var hardwareServiceOrderService = new Mock<IHardwareServiceOrderService>();

                    hardwareServiceOrderService.Setup(_ => 
                        _.GetCustomerServiceProvidersAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
                        .ReturnsAsync(new List<CustomerServiceProvider>());

                    services.AddSingleton(hardwareServiceOrderService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"origoapi/v1.0/backoffice/hardware-service/configuration/organization/{organizationId2}/service-provider");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
