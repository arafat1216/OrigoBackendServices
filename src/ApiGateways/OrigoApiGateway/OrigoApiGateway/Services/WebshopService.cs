using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class WebshopService : IWebshopService
    {
        private readonly ILogger<WebshopService> _logger;
        private readonly WebshopConfiguration _webshopConfiguration;
        private HttpClient _httpClient { get; }
        public WebshopService(ILogger<WebshopService> logger, IOptions<WebshopConfiguration> options, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _webshopConfiguration = options.Value;
        }

        public async Task ProvisionUserAsync(string email)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_webshopConfiguration.ApiPath}/users", email);
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
