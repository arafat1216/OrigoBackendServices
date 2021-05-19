using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, HttpClient httpClient, IOptions<AssetConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options.Value;
        }

        private ILogger<AssetServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly AssetConfiguration _options;


        public async Task<IList<OrigoAsset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var assets = await HttpClient.GetFromJsonAsync<IList<AssetDTO>>($"/Customers/{customerId}/Assets/{userId}");
                if (assets == null)
                {
                    return null;
                }
                var origoAssets = new List<OrigoAsset>();
                foreach (var asset in assets)
                {
                    origoAssets.Add(new OrigoAsset(asset));
                }
                return origoAssets;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with content type is not valid.");
                throw;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with invalid JSON.");
                throw;
            }
        }

        public async Task<IList<OrigoAsset>> GetAssetsForCustomerAsync(Guid customerId)
        {
            try
            {
                var assets = await HttpClient.GetFromJsonAsync<IList<AssetDTO>>($"/Customers/{customerId}/Assets");
                if (assets == null)
                {
                    return null;
                }
                var origoAssets = new List<OrigoAsset>();
                foreach (var asset in assets)
                {
                    origoAssets.Add(new OrigoAsset(asset));
                }
                return origoAssets;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with content type is not valid.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetsForUserAsync failed with invalid JSON.");
            }
            return null;
        }

        public async Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"/customers/{customerId}/assets", newAsset);
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentType is { MediaType: "application/json" })
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();

                        try
                        {
                            var asset = await JsonSerializer.DeserializeAsync<AssetDTO>(contentStream, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
                            return asset == null ? null : new OrigoAsset(asset);
                        }
                        catch (JsonException) // Invalid JSON
                        {
                            Console.WriteLine("Invalid JSON.");
                            return null;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }

            return null;
        }
    }
}