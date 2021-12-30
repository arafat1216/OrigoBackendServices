using AutoMapper;
using Common.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, HttpClient httpClient, IOptions<AssetConfiguration> options, IUserServices userServices, IMapper mapper)
        {
            _logger = logger;
            _daprClient = new DaprClientBuilder().Build();
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options.Value;
            _userServices = userServices;
            _mapper = mapper;
        }
        private readonly IUserServices _userServices;
        private readonly ILogger<AssetServices> _logger;
        private readonly IMapper _mapper;
        private readonly DaprClient _daprClient;
        private HttpClient HttpClient { get; }
        private readonly AssetConfiguration _options;

        public async Task<int> GetAssetsCountAsync(Guid customerId)
        {
            try
            {
                var count = await HttpClient.GetFromJsonAsync<int>($"{_options.ApiPath}/customers/{customerId}/count");

                return count;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetsCountAsync failed with HttpRequestException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetsCountAsync failed with unknown error.");
                throw;
            }
        }

        public async Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var assets = await HttpClient.GetFromJsonAsync<IList<AssetDTO>>($"{_options.ApiPath}/customers/{customerId}/users/{userId}");

                if (assets == null) return null;
                var origoAssets = new List<object>();
                foreach (var asset in assets)
                {
                    OrigoAsset result = null;
                    if (asset != null)
                    {
                        if (asset.AssetCategoryId == 1)
                            result = _mapper.Map<OrigoMobilePhone>(asset);
                        else
                            result = _mapper.Map<OrigoTablet>(asset);
                    }
                    origoAssets.Add(result);
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

        public async Task<IList<object>> GetAssetsForCustomerAsync(Guid customerId)
        {
            try
            {
                var assets = await HttpClient.GetFromJsonAsync<PagedAssetsDTO>($"{_options.ApiPath}/customers/{customerId}");

                if (assets == null)
                    return null;
                return assets.Assets;
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
                if (pagedAssetsDto == null)
                    return null;

                return _mapper.Map<OrigoPagedAssets>(pagedAssetsDto);
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
                var asset = await HttpClient.GetFromJsonAsync<AssetDTO>($"{_options.ApiPath}/{assetId}/customers/{customerId}");

                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);
                }
                return result;
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
                var requestUri = $"{_options.ApiPath}/customers/{customerId}";
                var response = await HttpClient.PostAsJsonAsync(requestUri, newAsset);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();

                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to save Asset.");
                throw;
            }
        }

        public async Task<IList<object>> UpdateStatusOnAssets(Guid customerId, UpdateAssetsStatus updatedAssetStatus, Guid callerId)
        {
            try
            {
                var updatedAssetStatusDTO = _mapper.Map<UpdateAssetsStatusDTO>(updatedAssetStatus);
                updatedAssetStatusDTO.CallerId = callerId; // Guid.Empty if tryparse fails.

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/assetStatus/{updatedAssetStatusDTO.AssetStatus.ToString().ToLower()}";
                //TODO: Why isn't Patch supported? Dapr translates it to POST.
                var response = await HttpClient.PostAsJsonAsync(requestUri, updatedAssetStatusDTO);
                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }

                var assets = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();
                List<object> origoAssets = new List<object>();
                if (assets == null)
                    return null;

                foreach (AssetDTO asset in assets)
                {
                    OrigoAsset result = null;
                    if (asset != null)
                    {
                        if (asset.AssetCategoryId == 1)
                            result = _mapper.Map<OrigoMobilePhone>(asset);
                        else
                            result = _mapper.Map<OrigoTablet>(asset);
                    }
                    origoAssets.Add(result);
                }

                return origoAssets;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to set status for assets.");
                throw;
            }
        }

        public async Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAsset updateAsset)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{assetId}/customers/{customerId}/update", updateAsset);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();
                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update asset.");
                throw;
            }
        }

        public async Task<IList<Label>> CreateLabelsForCustomerAsync(Guid customerId, AddLabelsData data)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels";

                var response = await HttpClient.PostAsJsonAsync(requestUri, data);

                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }
                var labels = await response.Content.ReadFromJsonAsync<IList<LabelDTO>>();
                return _mapper.Map<List<Label>>(labels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IList<Label>> GetCustomerLabelsAsync(Guid customerId)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels";
                var labels = await HttpClient.GetFromJsonAsync<IList<LabelDTO>>(requestUri);
                if (labels == null) return null;
                return _mapper.Map<List<Label>>(labels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IList<Label>> DeleteCustomerLabelsAsync(Guid customerId, DeleteCustomerLabelsData data)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/delete";
                var response = await HttpClient.PostAsJsonAsync(requestUri, data);
                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }
                var labels = await response.Content.ReadFromJsonAsync<IList<LabelDTO>>();
                return _mapper.Map<List<Label>>(labels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IList<Label>> UpdateLabelsForCustomerAsync(Guid customerId, UpdateCustomerLabelsData data)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/update";
                var response = await HttpClient.PostAsJsonAsync(requestUri, data);

                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }
                var labelsResult = await response.Content.ReadFromJsonAsync<IList<LabelDTO>>();
                return _mapper.Map<List<Label>>(labelsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<IList<object>> AssignLabelsToAssets(Guid customerId, AssetLabels assetLabels)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/assign";
                var response = await HttpClient.PostAsJsonAsync(requestUri, assetLabels);
                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }
                var assets = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();
                List<object> origoAssets = new List<object>();
                if (assets == null)
                    return null;

                foreach (AssetDTO asset in assets)
                {
                    OrigoAsset result = null;
                    if (asset != null)
                    {
                        if (asset.AssetCategoryId == 1)
                            result = _mapper.Map<OrigoMobilePhone>(asset);
                        else
                            result = _mapper.Map<OrigoTablet>(asset);
                    }
                    origoAssets.Add(result);
                }

                return origoAssets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IList<object>> UnAssignLabelsFromAssets(Guid customerId, AssetLabels assetLabels)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/unassign";
                var response = await HttpClient.PostAsJsonAsync(requestUri, assetLabels);
                if (!response.IsSuccessStatusCode)
                {
                    string errorDescription = await response.Content.ReadAsStringAsync();
                    if ((int)response.StatusCode == 500)
                        throw new Exception(errorDescription);
                    else if ((int)response.StatusCode == 404)
                        throw new ResourceNotFoundException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }

                var assets = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();
                List<object> origoAssets = new List<object>();
                if (assets == null)
                    return null;

                foreach (AssetDTO asset in assets)
                {
                    OrigoAsset result = null;
                    if (asset != null)
                    {
                        if (asset.AssetCategoryId == 1)
                            result = _mapper.Map<OrigoMobilePhone>(asset);
                        else
                            result = _mapper.Map<OrigoTablet>(asset);
                    }
                    origoAssets.Add(result);
                }

                return origoAssets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IList<OrigoAssetLifecycle>> GetLifecycles()
        {
            try
            {
                var lifecycles = await HttpClient.GetFromJsonAsync<IList<LifecycleDTO>>($"{_options.ApiPath}/lifecycles");
                if (lifecycles == null)
                    return null;

                return _mapper.Map<List<OrigoAssetLifecycle>>(lifecycles);
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

        public async Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, UpdateAssetLifecycleType data)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}/ChangeLifecycleType";
                var response = await HttpClient.PostAsJsonAsync(requestUri, data);
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

                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to change lifecycle type for asset.");
                throw;
            }
        }

        public async Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, AssignAssetToUser data)
        {
            try
            {
                if (data.UserId != null)
                {
                    try
                    {
                        var user = _userServices.GetUserAsync(customerId, data.UserId.Value).Result;
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

                var requestUri = $"{_options.ApiPath}/{data.AssetId}/customer/{customerId}/assign";
                var response = await HttpClient.PostAsJsonAsync(requestUri, data);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to assign asset", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to assign asset.");
                    throw exception;
                }
                var asset = await response.Content.ReadFromJsonAsync<AssetDTO>();

                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);
                }

                return result;
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
                var assetCategories = await HttpClient.GetFromJsonAsync<IList<AssetCategoryDTO>>($"{_options.ApiPath}/categories");
                if (assetCategories == null)
                    return null;

                return _mapper.Map<List<OrigoAssetCategory>>(assetCategories);
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetCategoriesAsync failed with unknown exception");
            }

            return null;
        }

        /// <summary>
        /// Get attributes for the asset category, along with wether it is required data or not.
        /// </summary>
        /// <param name="categoryId">The id (int) of the asset category, where 1 is MobilePhone and 2 is Tablet</param>
        /// <returns></returns>
        public IList<AssetCategoryAttribute> GetAssetCategoryAttributesForCategory(int categoryId)
        {
            List<AssetCategoryAttribute> defaultAttributes = new List<AssetCategoryAttribute>();

            // Check valid Id
            if (!(categoryId == 1 || categoryId == 2))
            {
                return defaultAttributes;
            }

            defaultAttributes.Add(new AssetCategoryAttribute { Name = "alias", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "assetCategoryId", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "note", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "brand", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "productName", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "lifecycleType", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "purchaseDate", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "managedByDepartmentId", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "assetHolderId", Required = false });

            if (categoryId == 1)
            { // MobilePhone - Mobiltelefon
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "imei", Required = true });
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "serialNumber", Required = false });
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "macAddress", Required = false });
            }
            else if (categoryId == 2)
            { // Tablet - Tablet? 
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "imei", Required = false });
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "serialNumber", Required = true });
                defaultAttributes.Add(new AssetCategoryAttribute { Name = "macAddress", Required = false });
            }


            return defaultAttributes;
        }

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId)
        {
            try
            {
                var assetLog =
               await HttpClient.GetFromJsonAsync<IList<AssetAuditLog>>(
                   $"{_options.ApiPath}/auditlog/{assetId}");


                foreach (AssetAuditLog log in assetLog)
                {
                    if (Guid.TryParse(log.CreatedBy, out Guid userId))
                    {
                        OrigoUser user = await _userServices.GetUserAsync(log.CustomerId, userId);
                        if (user != null)
                        {
                            log.CreatedBy = user.DisplayName;
                        }
                    }
                }

                return assetLog;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with content type is not valid.");
                throw;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetAssetAuditLogMock failed with invalid JSON.");
                throw;
            }
        }
    }
}