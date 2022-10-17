using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using OrigoApiGateway.Models;

namespace OrigoApiGateway.Services
{
    public class WebshopService : IWebshopService
    {
        private readonly ILogger<WebshopService> Logger;
        private readonly WebshopConfiguration _webshopConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
        public WebshopService(ILogger<WebshopService> logger, IOptions<WebshopConfiguration> options, IHttpClientFactory httpClientFactory)
        {
            Logger = logger;
            _httpClientFactory = httpClientFactory;
            _webshopConfiguration = options.Value;
        }

        public async Task ProvisionImplementUserAsync(string email)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_webshopConfiguration.ApiPath}/users", email);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ProvisionImplementUserAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task ProvisionUserWithEmployeeRoleAsync(Guid userId)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_webshopConfiguration.ApiPath}/provision", userId);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"ProvisionUserWithEmployeeRoleAsync failed with an exception of type {exception.GetType().Name}");
                throw;
            }
        }
    }
}
