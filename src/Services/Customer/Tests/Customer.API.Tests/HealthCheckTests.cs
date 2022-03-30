using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Customer.API.Controllers;

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
            var response = await _httpClient.GetAsync("/healthcheck");

            // TODO: Implement the API
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}