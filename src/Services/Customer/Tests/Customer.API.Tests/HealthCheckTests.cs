using Customer.API.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Customer.API.Tests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<OrganizationsController>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<OrganizationsController> factory)
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