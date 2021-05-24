using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
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

        private readonly ILogger<AssetServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly AssetConfiguration _options;


        public async Task<IList<OrigoAsset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var assets =
                    await HttpClient.GetFromJsonAsync<IList<AssetDTO>>(
                        $"{_options.ApiPath}/Customers/{customerId}/{userId}");
                if (assets == null) return null;
                var origoAssets = new List<OrigoAsset>();
                foreach (var asset in assets) origoAssets.Add(new OrigoAsset(asset));
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
                var assets =
                    await HttpClient.GetFromJsonAsync<IList<AssetDTO>>($"{_options.ApiPath}/Customers/{customerId}");
                if (assets == null) return null;
                var origoAssets = new List<OrigoAsset>();
                foreach (var asset in assets) origoAssets.Add(new OrigoAsset(asset));
                return origoAssets;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetsForCustomerAsync failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetsForCustomerAsync failed with content type is not valid.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetsForCustomerAsync failed with invalid JSON.");
            }

            return null;
        }

        public async Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/Customers/{customerId}", newAsset);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to save asset", (int) response.StatusCode);
                    _logger.LogError(exception, "Unable to save Asset.");
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to save Asset.");
                throw;
            }
        }
    }
}