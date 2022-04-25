using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace Common.Utilities;

public class ApiFeatureFilter : IFeatureDefinitionProvider
{
    private readonly HttpClient _httpClient;
    private readonly FeatureFlagConfiguration _options;

    private readonly IList<FeatureDefinition> _featureDefinitionList = new List<FeatureDefinition>();
    private bool _featureFlagsRead = false;

    private async Task SetFeatureFlagList()
    {
        if (!_featureFlagsRead)
        {
            var featureFlagList =
                await _httpClient.GetFromJsonAsync<List<string>>($"{_options.ApiPath}/feature-flags") ??
                new List<string>();
            foreach (var featureFlag in featureFlagList)
            {
                _featureDefinitionList.Add(new FeatureDefinition
                {
                    Name = featureFlag, EnabledFor = new[] { new FeatureFilterConfiguration { Name = "AlwaysOn" } }
                });
            }

            _featureFlagsRead = true;
        }
    }

    public ApiFeatureFilter(HttpClient httpClient, IOptions<FeatureFlagConfiguration> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        await SetFeatureFlagList();
        return _featureDefinitionList.FirstOrDefault(f => f.Name == featureName) ?? new FeatureDefinition();
    }

    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        await SetFeatureFlagList();
        foreach (var featureDefinition in _featureDefinitionList)
        {
            yield return await Task.FromResult(featureDefinition);
        }
    }
}