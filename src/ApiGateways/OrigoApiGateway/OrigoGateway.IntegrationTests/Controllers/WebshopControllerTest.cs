using System.Net.Http.Headers;
using System.Security.Claims;
using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;

namespace OrigoGateway.IntegrationTests.Controllers;

public class WebshopControllerTest : IClassFixture<OrigoGatewayWebApplicationFactory<WebshopController>>
{
    private readonly OrigoGatewayWebApplicationFactory<WebshopController> _factory;
    private readonly ITestOutputHelper _output;

    public WebshopControllerTest(OrigoGatewayWebApplicationFactory<WebshopController> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
        factory.ClientOptions.AllowAutoRedirect = false;
    }

    [Fact]
    public async Task ProvisionAuthenticatedUserIntoWebshop_With_WebshopAccess_Test()
    {
        var partnerId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "test@techstep.no";
        var permissionsIdentity = new ClaimsIdentity();
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(PredefinedRole.EndUser)));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, userId.ToString()));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
        permissionsIdentity.AddClaim(new Claim("Permissions", "WebshopAccess"));
        permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
        permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var userPermissionServiceMock = new Mock<IUserPermissionService>();
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None))
                    .Returns(Task.FromResult(permissionsIdentity));
                services.AddSingleton(userPermissionServiceMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                    options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                var webshopServiceMock = new Mock<IWebshopService>();
                webshopServiceMock.Setup(m => m.ProvisionUserWithEmployeeRoleAsync(It.IsAny<Guid>()));
                services.AddSingleton(webshopServiceMock.Object);
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
        
        var response = await client.PostAsync("/origoapi/v1.0/webshop/provision", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task ProvisionAuthenticatedUserIntoWebshop_Without_WebshopAccess_Test()
    {
        var partnerId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "test@techstep.no";
        var permissionsIdentity = new ClaimsIdentity();
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(PredefinedRole.EndUser)));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, userId.ToString()));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
        permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
        permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var userPermissionServiceMock = new Mock<IUserPermissionService>();
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None))
                    .Returns(Task.FromResult(permissionsIdentity));
                services.AddSingleton(userPermissionServiceMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                    options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                var webshopServiceMock = new Mock<IWebshopService>();
                webshopServiceMock.Setup(m => m.ProvisionUserWithEmployeeRoleAsync(It.IsAny<Guid>()));
                services.AddSingleton(webshopServiceMock.Object);
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
        
        var response = await client.PostAsync("/origoapi/v1.0/webshop/provision", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}