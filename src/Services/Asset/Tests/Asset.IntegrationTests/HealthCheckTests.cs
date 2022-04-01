using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Asset.API.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Asset.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<AssetsController>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<AssetsController> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/healthz");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}