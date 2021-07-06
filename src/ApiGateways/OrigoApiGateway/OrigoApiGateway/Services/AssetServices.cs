using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, HttpClient httpClient, IOptions<AssetConfiguration> options, IUserServices userServices)
        {
            _logger = logger;
            _daprClient = new DaprClientBuilder().Build();
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options.Value;
            _userServices = userServices;
        }
        private readonly IUserServices _userServices;
        private readonly ILogger<AssetServices> _logger;
        private readonly DaprClient _daprClient;
        private HttpClient HttpClient { get; }
        private readonly AssetConfiguration _options;


        public async Task<IList<OrigoAsset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var assets =
                    await HttpClient.GetFromJsonAsync<IList<AssetDTO>>(
                        $"{_options.ApiPath}/customers/{customerId}/users/{userId}");
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
                    await HttpClient.GetFromJsonAsync<PagedAssetsDTO>($"{_options.ApiPath}/customers/{customerId}");
                if (assets == null) return null;
                var origoAssets = new List<OrigoAsset>();
                foreach (var asset in assets.Assets) origoAssets.Add(new OrigoAsset(asset));
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

        public async Task<OrigoPagedAssets> SearchForAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 50)
        {
            try
            {
                var pagedAssetsDto = await HttpClient.GetFromJsonAsync<PagedAssetsDTO>($"{_options.ApiPath}/customers/{customerId}?q={search}&page={page}&limit={limit}");
                if (pagedAssetsDto == null) return null;

                var origoPagedAssets = new OrigoPagedAssets();
                foreach (var asset in pagedAssetsDto.Assets) origoPagedAssets.Assets.Add(new OrigoAsset(asset));
                origoPagedAssets.CurrentPage = pagedAssetsDto.CurrentPage;
                origoPagedAssets.TotalItems = pagedAssetsDto.TotalItems;
                origoPagedAssets.TotalPages = pagedAssetsDto.TotalPages;
                return origoPagedAssets;
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

        public async Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId)
        {
            try
            {
                var asset =
                    await HttpClient.GetFromJsonAsync<AssetDTO>($"{_options.ApiPath}/{assetId}/customers/{customerId}");
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetForCustomerAsync failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetForCustomerAsync failed with content type is not valid.");
            }

            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetForCustomerAsync failed with invalid JSON.");
            }

            return null;
        }

        public async Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/customers/{customerId}", newAsset);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to save asset", (int)response.StatusCode);
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

        public async Task<OrigoAsset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}/activate/{isActive.ToString().ToLower()}";
                //TODO: Why isn't Patch supported? Dapr translates it to POST.
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to set status for asset", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to set status for asset.");
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to set status for asset.");
                throw;
            }
        }

        public async Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAsset updateAsset)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{assetId}/customers/{customerId}/update/", updateAsset);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to update asset", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to update asset.");
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update asset.");
                throw;
            }
        }

        public async Task<IList<OrigoAssetLifecycle>> GetLifecycles()
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/lifecycles";
                var lifecycles = await HttpClient.GetFromJsonAsync<IList<LifecycleDTO>>(requestUri);
                if (lifecycles == null) return null;
                var origoAssets = new List<OrigoAssetLifecycle>();
                foreach (var lifecycle in lifecycles) origoAssets.Add(new OrigoAssetLifecycle() { Name = lifecycle.Name, EnumValue = lifecycle.EnumValue }); 
                return origoAssets;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetLifeCycles failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetLifeCycles failed with content type is not valid.");
            }

            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetLifecycles failed with invalid JSON.");
            }

            return null;
        }

        public async Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, int newLifecycleType)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}/ChangeLifecycleType/{newLifecycleType}";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    Exception exception;
                    if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                    {
                        exception = new InvalidLifecycleTypeException(response.ReasonPhrase);
                        _logger.LogError(exception, "Invalid lifecycletype given for asset.");
                    }
                    else
                    {
                        exception = new BadHttpRequestException("Unable to change lifecycle type for asset", (int)response.StatusCode);
                        _logger.LogError(exception, "Unable to change lifecycle type for asset.");
                    }

                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to change lifecycle type for asset.");
                throw;
            }
        }

        public async Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, Guid? userId)
        {
            try
            {
                if (userId != null)
                {
                    try
                    {
                        var user = _userServices.GetUserAsync(customerId, userId.Value).Result;
                        if (user == null)
                            throw new BadHttpRequestException("Unable to assign asset. User not found");
                    }
                    catch
                    {
                        var exception = new BadHttpRequestException("Unable to assign asset. User not found");
                        _logger.LogError(exception, "Unable to assign asset. User not found");
                        throw exception;
                    }
                }
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{assetId}/customer/{customerId}/assign?userId={userId}";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to assign asset", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to assign asset.");
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                return asset == null ? null : new OrigoAsset(asset);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to assign asset.");
                throw;
            }
        }

        public async Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync()
        {
            try
            {
                var assetCategories =
                      await HttpClient.GetFromJsonAsync<IList<AssetCategoryDTO>>(
                          $"{_options.ApiPath}/categories");
                if (assetCategories == null) return null;
                var origoAssetCategories = new List<OrigoAssetCategory>();
                foreach (var asset in assetCategories) origoAssetCategories.Add(new OrigoAssetCategory(asset));
                return origoAssetCategories;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetCategoriesAsync failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetCategoriesAsync failed with content type is not valid.");
            }

            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetCategoriesAsync failed with invalid JSON.");
            }

            return null;
        }

        public async Task<IList<string>> GetAssetAuditLog(Guid assetId)
        {
            try
            {
                var assetLog =
               await HttpClient.GetFromJsonAsync<IList<string>>(
                   $"{_options.ApiPath}/assets/auditlog?assetId={assetId}");

                return assetLog;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with content type is not valid.");
            }

            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with invalid JSON.");
            }
            return null;
        }
    }
}