using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrigoApiGateway.Services
{
    public class FeatureFlagServices : IFeatureFlagServices
    {
        private readonly ILogger<FeatureFlagServices> _logger;
        private readonly HttpClient _httpClient;
        private readonly FeatureFlagConfiguration _options;

        public FeatureFlagServices(ILogger<FeatureFlagServices> logger, HttpClient httpClient,
            IOptions<FeatureFlagConfiguration> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<IList<string>> GetFeatureFlags()
        {
            return await _httpClient.GetFromJsonAsync<List<string>>($"{_options.ApiPath}/feature-flags");
        }

        public async Task<IList<string>> GetFeatureFlags(Guid customerId)
        {
            return await _httpClient.GetFromJsonAsync<List<string>>($"{_options.ApiPath}/feature-flags/{customerId}");
        }

        public async Task AddFeatureFlags(string featureFlagName, Guid? customerId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_options.ApiPath}/feature-flags",
                new { featureFlagName, customerId });
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to add feature flag", (int)response.StatusCode);
        }

        public async Task DeleteFeatureFlags(string featureFlagName, Guid? customerId)
        {
            var requestUri = $"{_options.ApiPath}/feature-flags";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            httpRequestMessage.Content = customerId.HasValue
                ? JsonContent.Create(new { FeatureFlagName = featureFlagName, CustomerId = customerId })
                : JsonContent.Create(new { FeatureFlagName = featureFlagName });
            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to delete feature flag", (int)response.StatusCode);
        }
    }
}