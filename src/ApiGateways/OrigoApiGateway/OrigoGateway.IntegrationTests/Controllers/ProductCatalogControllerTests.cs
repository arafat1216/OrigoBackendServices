using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.ProductCatalog;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class ProductCatalogControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<ProductCatalogController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<ProductCatalogController> _factory;
        private readonly ITestOutputHelper _output;

        public ProductCatalogControllerTests(OrigoGatewayWebApplicationFactory<ProductCatalogController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            factory.ClientOptions.AllowAutoRedirect = false;
            _output = output;
        }

        [Fact]
        public async Task ReplaceOrderedProductsAsync_CustomerNotInPartner()
        {
            var partnerId = "f2993ee8-c37c-4697-a8cf-51254a785bbb";
            var email = "systemadmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, PredefinedRole.SystemAdmin.ToString()));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadAssetPermission"));



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

                    var productCatalogServices = new Mock<IProductCatalogServices>();
                    productCatalogServices.Setup(_ => _.ReplaceOrderedProductsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(),
                        It.IsAny<ProductOrdersDTO>()));
                    services.AddSingleton(productCatalogServices.Object);

                    var customerServices = new Mock<ICustomerServices>();
                    var customer = new Organization()
                    {
                        PartnerId = Guid.Empty
                    };
                    customerServices.Setup(_ => _.GetCustomerAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                        .Returns(Task.FromResult(customer));
                    services.AddSingleton(customerServices.Object);
                });
            }).CreateClient();

            var productOrder = new ProductOrdersPut()
            {
                ProductIds = new List<int>() { 1 }
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsync($"/origoapi/v1.0/products/partner/{partnerId}/organization/{Guid.NewGuid()}", JsonContent.Create(productOrder));
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task ReplaceOrderedProductsAsync()
        {
            var partnerId = "f2993ee8-c37c-4697-a8cf-51254a785bbb";
            var email = "systemadmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, PredefinedRole.SystemAdmin.ToString()));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadAssetPermission"));



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

                    var productCatalogServices = new Mock<IProductCatalogServices>();
                    productCatalogServices.Setup(_ => _.ReplaceOrderedProductsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(),
                        It.IsAny<ProductOrdersDTO>()));
                    services.AddSingleton(productCatalogServices.Object);

                    var customerServices = new Mock<ICustomerServices>();
                    var customer = new Organization()
                    {
                        PartnerId = Guid.Parse(partnerId)
                    };
                    customerServices.Setup(_ => _.GetCustomerAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                        .Returns(Task.FromResult(customer));
                    services.AddSingleton(customerServices.Object);
                });
            }).CreateClient();

            var productOrder = new ProductOrdersPut()
            {
                ProductIds = new List<int>() { 1 }
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsync($"/origoapi/v1.0/products/partner/{partnerId}/organization/{Guid.NewGuid()}", JsonContent.Create(productOrder));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
