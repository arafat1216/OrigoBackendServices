namespace Common.Infrastructure;

/// <summary>
/// Service used for Data caching
/// </summary>
public interface ICacheService
{

    /// <summary>
    /// Getting value from Cache(Redis) against a provided key
    /// </summary>
    /// <param name="key">key against which the value is fetched</param>
    /// <param name="stateStoreName"></param>
    /// <typeparam name="T">The fetched value will be serialized to type "T"</typeparam>
    /// <returns></returns>
    Task<T> Get<T>(string key, string stateStoreName = "statestore");

    /// <summary>
    /// Cache the value of type T against the provided key
    /// </summary>
    /// <param name="key">key against which the value is cached</param>
    /// <param name="value">The value of type T to be cached</param>
    /// <param name="ttl">Time-to-live (TTL) in seconds or the expiration time of the value</param>
    /// <param name="stateStoreName">metadata.name field in the user-configured statestore.yaml component file</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task Save<T>(string key, T value, string ttl, string stateStoreName = "statestore");


    /// <summary>
    /// Cache the list of values of type T against the provided key
    /// </summary>
    /// <param name="key">key against which the value is cached</param>
    /// <param name="value">The list of values of type T to be cached</param>
    /// <param name="ttl">Time-to-live (TTL) in seconds or the expiration time of the value</param>
    /// <param name="stateStoreName">metadata.name field in the user-configured statestore.yaml component file</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task Save<T>(string key, IEnumerable<T> value, string ttl, string stateStoreName = "statestore");


    /// <summary>
    /// Delete a cache for the provided key
    /// </summary>
    /// <param name="key">key of the value that needs to be deleted from cache</param>
    /// <param name="stateStoreName">metadata.name field in the user-configured statestore.yaml component file</param>
    /// <returns></returns>
    Task Delete(string key, string stateStoreName = "statestore");
}