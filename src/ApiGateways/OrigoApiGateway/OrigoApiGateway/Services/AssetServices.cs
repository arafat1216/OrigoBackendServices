﻿using Common.Models;
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
                    if (asset.AssetCategoryId == 1)
                        origoAssets.Add(new OrigoMobilePhone(asset));
                    else
                        origoAssets.Add(new OrigoTablet(asset));
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

                if (assets == null) return null;
                var origoAssets = new List<object>();
                foreach (var asset in assets.Assets)
                {
                    origoAssets.Add(asset);
                }
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
                foreach (var asset in pagedAssetsDto.Assets)
                {
                    origoPagedAssets.Assets.Add(asset);
                }
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
                var asset = await HttpClient.GetFromJsonAsync<AssetDTO>($"{_options.ApiPath}/{assetId}/customers/{customerId}");

                OrigoAsset result = null;
                if (asset == null)
                    return result;
                if (asset.AssetCategoryId == 1)
                    result = new OrigoMobilePhone(asset);
                else
                    result = new OrigoTablet(asset);

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
                if (asset == null)
                    return result;
                if (asset.AssetCategoryId == 1)
                    result = new OrigoMobilePhone(asset);
                else
                    result = new OrigoTablet(asset);

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to save Asset.");
                throw;
            }
        }

        public async Task<IList<object>> UpdateStatusOnAssets(Guid customerId, UpdateAssetsStatus data, int assetStatus)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/assetStatus/{assetStatus.ToString().ToLower()}";
                //TODO: Why isn't Patch supported? Dapr translates it to POST.
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

                var assets = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();
                List<object> origoAssets = new List<object>();
                if (assets == null)
                    return null;

                foreach (AssetDTO asset in assets)
                {
                    if (asset == null)
                        continue;
                    OrigoAsset result;
                    if (asset.AssetCategoryId == 1)
                        result = new OrigoMobilePhone(asset);
                    else
                        result = new OrigoTablet(asset);
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
                if (asset == null)
                    return result;
                if (asset.AssetCategoryId == 1)
                    result = new OrigoMobilePhone(asset);
                else
                    result = new OrigoTablet(asset);

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

                var labels = await response.Content.ReadFromJsonAsync<IList<Label>>();

                return labels;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<Label>> GetCustomerLabelsAsync(Guid customerId)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels";
                var labels = await HttpClient.GetFromJsonAsync<IList<Label>>(requestUri);
                if (labels == null) return null;
                return labels;
            }
            catch(Exception ex)
            {
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

                var labels = await response.Content.ReadFromJsonAsync<IList<Label>>();

                return labels;
            }
            catch (Exception ex)
            {
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

                var labelsResult = await response.Content.ReadFromJsonAsync<IList<Label>>();

                return labelsResult;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<object>> AssignLabelsToAssets(Guid customerId, AssetLabels assetLabels)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/assign";
                var response = await HttpClient.PostAsJsonAsync<AssetLabels>(requestUri, assetLabels);
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
                    if (asset == null)
                        continue;
                    OrigoAsset result;
                    if (asset.AssetCategoryId == 1)
                        result = new OrigoMobilePhone(asset);
                    else
                        result = new OrigoTablet(asset);
                    origoAssets.Add(result);
                }

                return origoAssets;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<object>> UnAssignLabelsFromAssets(Guid customerId, AssetLabels assetLabels)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/customers/{customerId}/labels/unassign";
                var response = await HttpClient.PostAsJsonAsync<AssetLabels>(requestUri, assetLabels);
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
                    if (asset == null)
                        continue;
                    OrigoAsset result;
                    if (asset.AssetCategoryId == 1)
                        result = new OrigoMobilePhone(asset);
                    else
                        result = new OrigoTablet(asset);
                    origoAssets.Add(result);
                }

                return origoAssets;
            }
            catch (Exception ex)
            {
                return null;
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
                if (asset == null)
                    return result;
                if (asset.AssetCategoryId == 1)
                    result = new OrigoMobilePhone(asset);
                else
                    result = new OrigoTablet(asset);

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
                if (asset == null)
                    return result;
                if (asset.AssetCategoryId == 1)
                    result = new OrigoMobilePhone(asset);
                else
                    result = new OrigoTablet(asset);

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
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetCategoriesAsync failed with unknown exception");
            }

            return null;
        }

        public IList<AssetCategoryAttribute> GetAssetCategoryAttributesForCategory(int categoryId)
        {
            List<AssetCategoryAttribute> defaultAttributes = new List<AssetCategoryAttribute>();
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "Alias",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "AssetCategoryId",
                Required = true
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "Note",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "Brand",
                Required = true
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "ProductName",
                Required = true
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "LifecycleType",
                Required = true
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "PurchaseDate",
                Required = true
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "ManagedByDepartmentId",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "AssetHolderId",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "Description",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "AssetTag",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "Imei",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "MacAddress",
                Required = false
            });
            defaultAttributes.Add(new AssetCategoryAttribute
            {
                Name = "SerialNumber",
                Required = false
            });
            return defaultAttributes;
            /*if (categoryId == 1) // MobilePhone - Mobiltelefon
            {
                defaultAttributes.Add(new AssetCategoryAttribute
                {
                    Name = "Imei",
                    Required = true
                });
            }*/
        }

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId)
        {
            try
            {
                var assetLog =
               await HttpClient.GetFromJsonAsync<IList<AssetAuditLog>>(
                   $"{_options.ApiPath}/auditlog/{assetId}");

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