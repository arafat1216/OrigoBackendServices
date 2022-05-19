using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using Xunit;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class AssetControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<AssetsController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<AssetsController> _factory;
        private readonly HttpClient _httpClient;

        public AssetControllerTests(OrigoGatewayWebApplicationFactory<AssetsController> factory)
        {
            _factory = factory;
            factory.ClientOptions.AllowAutoRedirect = false;
            _httpClient = factory.CreateDefaultClient();
        }

        public static IEnumerable<object[]> EmailAccess => new List<object[]>
        {
            new object[] { "unknown@test.io", HttpStatusCode.Forbidden },
            new object[] { "admin@test.io", HttpStatusCode.Forbidden },
            new object[] { "systemadmin@test.io", HttpStatusCode.OK }
        };

        [Theory]
        [MemberData(nameof(EmailAccess))]
        public async Task Get_SecurePageAccessibleOnlyByAdminUsers(string email, HttpStatusCode expected)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                    var assetService = new Mock<IAssetServices>();
                    var customerAssetCount = new List<CustomerAssetCount> { new() { OrganizationId = Guid.NewGuid(), Count = 12 } };
                    assetService.Setup(_ => _.GetAllCustomerAssetsCountAsync()).Returns(Task.FromResult(customerAssetCount as IList<CustomerAssetCount>));
                    services.AddSingleton(assetService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            var response = await client.GetAsync("/origoapi/v1.0/assets/customers/count");

            Assert.Equal(expected, response.StatusCode);
        }
    }
}
