using OrigoApiGateway;

namespace OrigoGateway.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<OrigoGatewayWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        public HealthCheckTests(OrigoGatewayWebApplicationFactory<Startup> factory)
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