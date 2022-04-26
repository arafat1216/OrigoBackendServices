using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Asset.IntegrationTests;

public class FeatureManagerStub : IFeatureManager
{
    public async IAsyncEnumerable<string> GetFeatureNamesAsync()
    {
        foreach (var featureDefinition in FeatureList)
        {
            yield return await Task.FromResult(featureDefinition);
        }
    }

    public Task<bool> IsEnabledAsync(string feature)
    {
        return Task.FromResult(FeatureList.Contains(feature));
    }

    public Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
    {
        return Task.FromResult(FeatureList.Contains(feature));
    }

    public IList<string> FeatureList { get; set; } = new List<string>();
}