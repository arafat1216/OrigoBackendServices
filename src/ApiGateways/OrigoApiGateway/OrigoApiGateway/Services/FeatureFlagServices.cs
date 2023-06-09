﻿using Microsoft.Extensions.Options;
using Common.Utilities;

namespace OrigoApiGateway.Services
{
    public class FeatureFlagServices : IFeatureFlagServices
    {
        private readonly ILogger<FeatureFlagServices> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
        private readonly FeatureFlagConfiguration _options;

        public FeatureFlagServices(ILogger<FeatureFlagServices> logger, IHttpClientFactory httpClientFactory,
            IOptions<FeatureFlagConfiguration> options)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<IList<string>> GetFeatureFlags()
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{_options.ApiPath}/feature-flags");
        }

        public async Task<IList<string>> GetFeatureFlags(Guid customerId)
        {
            return await HttpClient.GetFromJsonAsync<List<string>>($"{_options.ApiPath}/feature-flags/{customerId}");
        }

        public async Task AddFeatureFlags(string featureFlagName, Guid? customerId)
        {
            var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/feature-flags",
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
            var response = await HttpClient.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to delete feature flag", (int)response.StatusCode);
        }
    }
}