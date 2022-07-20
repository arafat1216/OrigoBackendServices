using Microsoft.Extensions.Options;

namespace OrigoApiGateway.Services
{
    public class WebshopService : IWebshopService
    {
        private readonly ILogger<WebshopService> _logger;
        private readonly WebshopConfiguration _webshopConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
        public WebshopService(ILogger<WebshopService> logger, IOptions<WebshopConfiguration> options, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _webshopConfiguration = options.Value;
        }

        public async Task ProvisionUserAsync(string email)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_webshopConfiguration.ApiPath}/users", email);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProvisionUserAsync failed with HttpRequestException.");
                throw;
            }
        }
    }
}
