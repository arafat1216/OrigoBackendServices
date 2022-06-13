using AutoMapper;
using Common.Enums;
using Common.Interfaces;
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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace OrigoApiGateway.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, HttpClient httpClient, IOptions<AssetConfiguration> options, IUserServices userServices, IMapper mapper, IDepartmentsServices departmentsServices)
        {
            _logger = logger;
            _daprClient = new DaprClientBuilder().Build();
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options.Value;
            _userServices = userServices;
            _departmentsServices = departmentsServices;
            _mapper = mapper;
        }
        private readonly IUserServices _userServices;
        private readonly IDepartmentsServices _departmentsServices;
        private readonly ILogger<AssetServices> _logger;
        private readonly IMapper _mapper;
        private readonly DaprClient _daprClient;
        private HttpClient HttpClient { get; }
        private readonly AssetConfiguration _options;

        public async Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync()
        {
            try
            {
                IList<CustomerAssetCount> assetCountList = await HttpClient.GetFromJsonAsync<IList<CustomerAssetCount>>($"{_options.ApiPath}/customers/count");

                return assetCountList;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAllCustomerAssetsCountAsync failed with HttpRequestException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAllCustomerAssetsCountAsync failed with unknown error.");
                throw;
            }
        }

        public async Task<int> GetAssetsCountAsync(Guid customerId, Guid? departmentId,
            AssetLifecycleStatus? assetLifecycleStatus)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/count";
                if (departmentId != null && departmentId != Guid.Empty)
                {
                    requestUri = QueryHelpers.AddQueryString(requestUri, "departmentId", $"{departmentId}");
                }

                if (assetLifecycleStatus.HasValue)
                {
                    requestUri = QueryHelpers.AddQueryString($"{_options.ApiPath}/customers/{customerId}/count",
                        "assetLifecycleStatus", $"{(int)assetLifecycleStatus}");
                }

                var count = await HttpClient.GetFromJsonAsync<int>(requestUri);

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

        public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
        {
            try
            {
                var totalBookValue = await HttpClient.GetFromJsonAsync<decimal>($"{_options.ApiPath}/customers/{customerId}/total-book-value");

                return totalBookValue;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetCustomerTotalBookValue failed with HttpRequestException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomerTotalBookValue failed with unknown error.");
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

        public async Task<PagedModel<HardwareSuperType>> GetAssetsForCustomerAsync(Guid customerId, FilterOptionsForAsset filterOptions, string search = "", int page = 1, int limit = 1000)
        {
            try
            {
                string json = JsonSerializer.Serialize(filterOptions);
                var assets = await HttpClient.GetFromJsonAsync<PagedModel<HardwareSuperType>>($"{_options.ApiPath}/customers/{customerId}?q={search}&page={page}&limit={limit}&filterOptions={json}");

                if (assets == null)
                    return null;

                foreach (var asset in assets.Items)
                {
                    try
                    {
                        if (asset.ManagedByDepartmentId != null)
                        {
                            var department = await _departmentsServices.GetDepartmentAsync(asset.OrganizationId, asset.ManagedByDepartmentId ?? throw new ArgumentNullException("DepartmentId"));
                            if (department != null) asset.DepartmentName = department.Name;
                        }
                        if (asset.AssetHolderId != null)
                        {
                            var user = await _userServices.GetUserAsync(asset.AssetHolderId ?? throw new ArgumentNullException("UserId"));
                            if (user != null) asset.AssetHolderName = user.DisplayName;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Department or User fetch in GetAssetsForCustomerAsync failed with HttpRequestException.");

                    }
                    catch (ArgumentNullException ex)
                    {
                        _logger.LogError($"{ex.Message} was null in GetAssetsForCustomerAsync failed with HttpRequestException.");

                    }
                }

                return assets;
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
            catch (ResourceNotFoundException exception)
            {
                _logger.LogError(exception, "GetAssetsForCustomerAsync failed with invalid JSON.");
            }

            return null;
        }

        public async Task<IList<LifeCycleSetting>> GetLifeCycleSettingByCustomer(Guid customerId, string currency)
        {
            try
            {
                var settings = await HttpClient.GetFromJsonAsync<IList<LifeCycleSetting>>($"{_options.ApiPath}/customers/{customerId}/lifecycle-setting");

                if (settings == null)
                    return null;
                foreach(var setting in settings)
                {
                    setting.Currency = currency;
                }
                return settings;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetLifeCycleSettingByCustomer failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetLifeCycleSettingByCustomer failed with content type is not valid.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetLifeCycleSettingByCustomer failed with invalid JSON.");
            }

            return null;
        }

        public async Task<LifeCycleSetting> SetLifeCycleSettingForCustomerAsync(Guid customerId, NewLifeCycleSetting setting, string currency, Guid callerId)
        {
            try
            {
                var existingSetting = await GetLifeCycleSettingByCustomer(customerId, currency);
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/lifecycle-setting";
                var response = new HttpResponseMessage();
                var newSettingtDTO = _mapper.Map<NewLifeCycleSettingDTO>(setting);
                newSettingtDTO.CallerId = callerId;
                if (existingSetting == null || existingSetting!.FirstOrDefault(x=>x.AssetCategoryId == setting.AssetCategoryId) == null)
                    response = await HttpClient.PostAsJsonAsync(requestUri, newSettingtDTO);
                else
                    response = await HttpClient.PutAsJsonAsync(requestUri, newSettingtDTO);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var newSetting = await response.Content.ReadFromJsonAsync<LifeCycleSetting>();
                newSetting.Currency = currency;
                return newSetting;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to set LifeCycleSetting.");
                throw;
            }
        }

        public async Task<DisposeSetting> GetDisposeSettingByCustomer(Guid customerId)
        {
            try
            {
                var settings = await HttpClient.GetFromJsonAsync<DisposeSetting>($"{_options.ApiPath}/customers/{customerId}/dispose-setting");

                if (settings == null)
                    return null;

                return settings;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetDisposeSettingByCustomer failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetDisposeSettingByCustomer failed with content type is not valid.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetDisposeSettingByCustomer failed with invalid JSON.");
            }

            return null;
        }

        public async Task<DisposeSetting> SetDisposeSettingForCustomerAsync(Guid customerId, NewDisposeSetting setting, Guid callerId)
        {
            try
            {
                var existingSetting = await GetDisposeSettingByCustomer(customerId);
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/dispose-setting";
                var response = new HttpResponseMessage();
                var newSettingtDTO = _mapper.Map<NewDisposeSettingDTO>(setting);
                newSettingtDTO.CallerId = callerId;
                if (existingSetting == null)
                    response = await HttpClient.PostAsJsonAsync(requestUri, newSettingtDTO);
                else
                    response = await HttpClient.PutAsJsonAsync(requestUri, newSettingtDTO);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var newSetting = await response.Content.ReadFromJsonAsync<DisposeSetting>();
                return newSetting;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to set DisposeSetting.");
                throw;
            }
        }

        public async Task<HardwareSuperType> GetAssetAsync(Guid customerId, Guid assetId)
        {
            try
            {
                var asset = await HttpClient.GetFromJsonAsync<AssetDTO>($"{_options.ApiPath}/{assetId}/customers/{customerId}");

                HardwareSuperType result = null;
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

        public async Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAssetDTO newAsset)
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
        public async Task<OrigoAsset> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailable data, Guid callerId)
        {
            try
            {
                var makeAssetAvailableDTO = _mapper.Map<MakeAssetAvailableDTO>(data);
                makeAssetAvailableDTO.CallerId = callerId; // Guid.Empty if tryparse fails.

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/make-available";
                var existingAsset = await GetAssetForCustomerAsync(customerId, data.AssetLifeCycleId);
                if(existingAsset.AssetHolderId != null)
                {
                    var user = await _userServices.GetUserAsync(existingAsset.AssetHolderId.Value);
                    makeAssetAvailableDTO.PreviousUser = new EmailPersonAttributeDTO()
                    {
                        Email = user.Email,
                        Name = user.FirstName,
                        PreferedLanguage = user.UserPreference.Language
                    };
                }

                var response = await HttpClient.PostAsJsonAsync(requestUri, makeAssetAvailableDTO);
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
                _logger.LogError(exception, "Unable to set status for assets.");
                throw;
            }
        }

        public async Task<OrigoAsset> ReAssignAssetToDepartment(Guid customerId, Guid assetId, ReassignedToDepartmentDTO data)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetId);
                var department = await _departmentsServices.GetDepartmentAsync(customerId, existingAsset.ManagedByDepartmentId.Value);
                var oldManagers = new List<EmailPersonAttributeDTO>();
                foreach (var manager in department.ManagedBy)
                {
                    var oldManager = await _userServices.GetUserAsync(customerId, manager.UserId);
                    oldManagers.Add(new EmailPersonAttributeDTO()
                    {
                        Name = oldManager.FirstName,
                        Email = oldManager.Email,
                        PreferedLanguage = oldManager.UserPreference!.Language
                    });
                }
                data.PreviousManagers = oldManagers;
                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}/re-assignment";
                var response = await HttpClient.PostAsJsonAsync(requestUri, JsonContent.Create(data));

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
                _logger.LogError(exception, "Unable to re-assign the asset");
                throw;
            }
        }

        public async Task<OrigoAsset> ReAssignAssetToUser(Guid customerId, Guid assetId, ReassignedToUserDTO data)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetId);
                if (existingAsset.AssetHolderId != null)
                {
                    var previousUser = await _userServices.GetUserAsync(existingAsset.AssetHolderId.Value);
                    data.PreviousUser = new EmailPersonAttributeDTO()
                    {
                        Name = previousUser.FirstName,
                        Email = previousUser.Email,
                        PreferedLanguage = previousUser.UserPreference!.Language
                    };
                }
                var department = await _departmentsServices.GetDepartmentAsync(customerId, existingAsset.ManagedByDepartmentId.Value);
                var oldManagers = new List<EmailPersonAttributeDTO>();
                foreach(var manager in department.ManagedBy)
                {
                    var oldManager = await _userServices.GetUserAsync(customerId, manager.UserId);
                    oldManagers.Add(new EmailPersonAttributeDTO()
                    {
                        Name = oldManager.FirstName,
                        Email = oldManager.Email,
                        PreferedLanguage = oldManager.UserPreference!.Language
                    });
                }
                data.PreviousManagers = oldManagers;
                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}/re-assignment";
                var response = await HttpClient.PostAsJsonAsync(requestUri, JsonContent.Create(data));

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
                _logger.LogError(exception, "Unable to re-assign the asset");
                throw;
            }
        }

        public async Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAssetDTO updateAsset)
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

        public async Task<IList<object>> AssignLabelsToAssets(Guid customerId, AssetLabelsDTO assetLabels)
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

        public async Task<IList<object>> UnAssignLabelsFromAssets(Guid customerId, AssetLabelsDTO assetLabels)
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

        public async Task<IList<MinBuyoutPrice>> GetBaseMinBuyoutPrice(string? country = null, int? assetCategoryId = null)
        {
            try
            {
                var allMinBuyoutPrices = await HttpClient.GetFromJsonAsync<IList<MinBuyoutPrice>>($"{_options.ApiPath}/min-buyout-price?{(string.IsNullOrWhiteSpace(country) ? "" : $"country={country}")}&{(assetCategoryId == null ? "" : $"assetCategoryId={assetCategoryId}")}");
                if (allMinBuyoutPrices == null)
                    return null;
                foreach(var data in allMinBuyoutPrices)
                {
                    data.Currency = GetCurrencyByCountry(data.Country);
                }
                return allMinBuyoutPrices;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetBaseMinBuyoutPrice failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetBaseMinBuyoutPrice failed with content type is not valid.");
            }

            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetBaseMinBuyoutPrice failed with invalid JSON.");
            }

            return null;
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

        public string GetCurrencyByCountry(string? country = null)
        {
            if (string.IsNullOrEmpty(country))
                return CurrencyCode.NOK.ToString();

            switch (country.ToUpper().Trim())
            {
                case "NORWAY":
                    return CurrencyCode.NOK.ToString();
                case "SWEDEN":
                    return CurrencyCode.SEK.ToString();
                case "DENMARK":
                    return CurrencyCode.DKK.ToString();
                case "UNITED STATES":
                    return CurrencyCode.USD.ToString();
                default:
                    return CurrencyCode.EUR.ToString();
            };
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

        public async Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, AssignAssetToUser assignedAsset)
        {
            try
            {
                if (assignedAsset.UserId != Guid.Empty)
                {
                    try
                    {
                        var user = await _userServices.GetUserAsync(customerId, assignedAsset.UserId);
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
                if (assignedAsset.DepartmentId != Guid.Empty)
                {
                    try
                    {
                        var department = await _departmentsServices.GetDepartmentAsync(customerId, assignedAsset.DepartmentId);
                        if (department == null)
                            throw new BadHttpRequestException("Unable to assign asset. Department not found");
                    }
                    catch
                    {
                        var exception = new BadHttpRequestException("Unable to assign asset. Department not found");
                        _logger.LogError(exception, "Unable to assign asset. Department not found");
                        throw exception;
                    }
                }

                var requestUri = $"{_options.ApiPath}/{assignedAsset.AssetId}/customer/{customerId}/assign";
                var response = await HttpClient.PostAsJsonAsync(requestUri, assignedAsset);
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

        public async Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync(string? language = "EN")
        {
            try
            {
                var assetCategories = await HttpClient.GetFromJsonAsync<IList<AssetCategoryDTO>>($"{_options.ApiPath}/categories?language={language}");
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
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "createdDate", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "startPeriod", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "endPeriod", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "paidByCompany", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "bookValue", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "buyoutPrice", Required = false });

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

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId,Guid callerId, string role)
        {
            try
            {
                var assetLog = await HttpClient.GetFromJsonAsync<IList<AssetAuditLog>>($"{_options.ApiPath}/auditlog/{assetId}/{callerId}/{role}");
                if (assetLog == null || !assetLog.Any()) return assetLog;

                IList<OrigoUser> users = new List<OrigoUser>();
                var organizationId = assetLog.First().CustomerId;
                foreach (var createdBy in assetLog.Select(s => s.CreatedBy).Distinct())
                {
                    if (!Guid.TryParse(createdBy, out var userId)) continue;
                    var user = await _userServices.GetUserAsync(userId);
                    if (user == null)
                        continue;
                    users.Add(user);
                }
                foreach (var log in assetLog)
                {
                    if (!Guid.TryParse(log.CreatedBy, out var userId)) continue;
                    var user = users.FirstOrDefault(u => u.Id == userId);
                    log.CreatedBy = user?.DisplayName ?? "";
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
        public async Task<string> CreateAssetSeedData()
        {
            try
            {
                var errorMessage = await HttpClient.GetStringAsync($"{_options.ApiPath}/seed");
                return errorMessage;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "CreateOrganizationSeedData failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "CreateOrganizationSeedData failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateOrganizationSeedData unknown error.");
                throw;
            }
        }

        public async Task<OrigoCustomerAssetsCounter> GetAssetLifecycleCountersAsync(Guid customerId, FilterOptionsForAsset filter, bool manager)
        {

            try
            {
                List<Guid> managerOfDepartments = null;
                
                if (Guid.TryParse(filter.UserId, out Guid userId))
                {
                    try
                    {

                        var user = await _userServices.GetUserAsync(customerId, userId);
                        if (user == null)
                            throw new BadHttpRequestException("User not found");

                        if (manager)
                        {
                            managerOfDepartments = user.ManagerOf.Select(a => a.DepartmentId).ToList();
                            if (!managerOfDepartments.Any()) return new OrigoCustomerAssetsCounter();
                        }

                    }
                    catch
                    {
                        var exception = new BadHttpRequestException("User not found");
                        _logger.LogError(exception, exception.Message);
                        throw exception;
                    }
                }

                try
                {
                    if(managerOfDepartments != null && managerOfDepartments.Any())
                    {
                        if (filter.Department != null && filter.Department.Any())
                        {
                            //Make department list based on manager accsess to and only the one's that is in filter    
                            var guids = filter.Department.OfType<Guid>().ToList();
                            managerOfDepartments = managerOfDepartments.Intersect(guids).ToList();
                        }

                        foreach (var department in managerOfDepartments)
                        {
                            try
                            {
                                var isDepartment = await _departmentsServices.GetDepartmentAsync(customerId, department);
                                if (isDepartment != null)
                                {
                                    if(filter.Department == null) filter.Department = new List<Guid?>();
                                    filter.Department.Add(department);
                                }
                            }
                            catch
                            {
                                //only catch and do nothing
                            }
                        }

                    }
                }
                catch
                {
                    var exception = new BadHttpRequestException("Department ids dont match what user have accsess to");
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }

                string json = JsonSerializer.Serialize(filter);
              

                return await HttpClient.GetFromJsonAsync<OrigoCustomerAssetsCounter>($"{_options.ApiPath}/customers/{customerId}/assets-counter/?filter={json}");


            }
            catch (InvalidUserValueException exception)
            {
                throw;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetsCount failed with HttpRequestException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetsCount failed with Exception.");
                throw;
            }

        }
    }
}