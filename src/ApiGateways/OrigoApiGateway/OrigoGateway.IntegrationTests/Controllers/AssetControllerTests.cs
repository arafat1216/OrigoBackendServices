using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class AssetControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<OrigoApiGateway.Controllers.AssetsController>>
    {
        private readonly HttpClient _httpClient;

        public AssetControllerTests(OrigoGatewayWebApplicationFactory<OrigoApiGateway.Controllers.AssetsController> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task GetDepartments()
        {
            var response = await _httpClient.GetAsync($"/origoapi/v1.0/assets/customers/count");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
