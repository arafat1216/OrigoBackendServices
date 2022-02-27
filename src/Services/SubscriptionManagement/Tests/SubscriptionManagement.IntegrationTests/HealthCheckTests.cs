using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionManagement.API.Controllers;
using Xunit;

namespace SubscriptionManagement.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<SubscriptionManagementController>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<SubscriptionManagementController> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync(("/healthcheck"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}