using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductCatalog.API.Controllers;
using Xunit;

namespace ProductCatalog.IntegrationTests;

public class HealthCheckTests : IClassFixture<WebApplicationFactory<ProductsController>>
{
    private readonly HttpClient _httpClient;

    public HealthCheckTests(WebApplicationFactory<ProductsController> factory)
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