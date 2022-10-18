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
    public async Task<T?> Get<T>(string key, string stateStoreName = "statestore")
    {
        try
        {
            return await _daprClient.GetStateAsync<T>(stateStoreName, key);
        }
        catch (Exception e)
        {
            return default(T);
        }
    }

    /// <inheritdoc />
    public async Task Save<T>(string key, T value, string? ttl, string stateStoreName = "statestore")
    {
        try
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
        catch (Exception e)
        {
            
        }
    }

    /// <inheritdoc />
    public async Task Save<T>(string key, IEnumerable<T> value, string? ttl, string stateStoreName = "statestore")
    {
        try
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
        catch (Exception e)
        {

        }
    }

    /// <inheritdoc />
    public async Task Delete(string key, string stateStoreName = "statestore")
    {
        try
        {
            await _daprClient.DeleteStateAsync(stateStoreName, key);
        }
        catch (Exception e)
        {

        }
    }
    
    
}