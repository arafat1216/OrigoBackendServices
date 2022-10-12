using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class HardwareServiceCustomerControllerTest : IClassFixture<OrigoGatewayWebApplicationFactory<HardwareServiceCustomerController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<HardwareServiceCustomerController> _factory;
        private readonly ITestOutputHelper _output;

        public HardwareServiceCustomerControllerTest(OrigoGatewayWebApplicationFactory<HardwareServiceCustomerController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }

        [Fact]
        public async Task CreateHardwareServiceOrderForRemarketingAsync_Test_For_PartnerAdmin()
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
            var hardwareServiceOrder = new NewHardwareServiceOrder()
            {
                DeliveryAddress = new () {
                    RecipientType = RecipientTypeEnum.Personal,
                    Recipient = "[Recipient]",
                    Address1 = "[Address1]",
                    Address2 = "[Address2]",
                    PostalCode = "[0001]",
                    City = "[Oslo]",
                    Country = "NO"
                },
                UserDescription = "[UserDescription]",
                AssetId = Guid.NewGuid(),
                UserSelectedServiceOrderAddonIds = new HashSet<int>() { 1 }
            };
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

                    var assetService = new Mock<IAssetServices>();

                    var hardwareSuperType = new HardwareSuperType()
                    {
                        AssetHolderId = Guid.NewGuid(),
                        ManagedByDepartmentId = Guid.NewGuid()
                    };

                    assetService.Setup(_ => _.GetAssetForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), null,
                        It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                        .ReturnsAsync(hardwareSuperType);
                    
                    var hardwareServiceOrderService = new Mock<IHardwareServiceOrderService>();

                    hardwareServiceOrderService.Setup(_ => 
                        _.CreateHardwareServiceOrderAsync(
                            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>(), 
                            It.IsAny<OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request.NewHardwareServiceOrder>(), 
                            It.IsAny<HardwareSuperType>()))
                        .ReturnsAsync(new OrigoApiGateway.Models.HardwareServiceOrder.Backend.HardwareServiceOrder() { });

                    services.AddSingleton(assetService.Object);
                    services.AddSingleton(hardwareServiceOrderService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsJsonAsync($"origoapi/v1.0/hardware-service/organization/{organizationId2}/orders/remarketing", hardwareServiceOrder);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task GetServiceOrderByIdAndOrganizationAsync_Test_For_PartnerAdmin()
        {
            var serviceOrderId = Guid.NewGuid();
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
                        _.GetServiceOrderByIdAndOrganizationAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                        .ReturnsAsync(new OrigoApiGateway.Models.HardwareServiceOrder.Backend.HardwareServiceOrder() { });

                    services.AddSingleton(hardwareServiceOrderService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"origoapi/v1.0/hardware-service/organization/{organizationId2}/orders/{serviceOrderId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
