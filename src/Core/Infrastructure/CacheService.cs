using Dapr.Client;

namespace Common.Infrastructure;

/// <inheritdoc />
public class CacheService : ICacheService
{
    private readonly DaprClient _daprClient;

    public CacheService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    /// <inheritdoc />
    public async Task<T> Get<T>(string key, string stateStoreName = "statestore")
    {
        return await _daprClient.GetStateAsync<T>(stateStoreName, key);
    }

    /// <inheritdoc />
    public async Task Save<T>(string key, T value, string? ttl, string stateStoreName = "statestore")
    {
        if (ttl is not null)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.TryAdd("ttlInSeconds", ttl);
            await _daprClient.SaveStateAsync(stateStoreName, key, value, metadata: metadata);
        }
        else
        {
            await _daprClient.SaveStateAsync(stateStoreName, key, value);
        }
    }

    /// <inheritdoc />
    public async Task Save<T>(string key, IEnumerable<T> value, string? ttl, string stateStoreName = "statestore")
    {
        if (ttl is not null)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.TryAdd("ttlInSeconds", ttl);
            await _daprClient.SaveStateAsync(stateStoreName, key, value, metadata: metadata);
        }
        else
        {
            await _daprClient.SaveStateAsync(stateStoreName, key, value);
        }
    }

    /// <inheritdoc />
    public async Task Delete(string key, string stateStoreName = "statestore")
    {
        await _daprClient.DeleteStateAsync(stateStoreName, key);
    }
    
    
}