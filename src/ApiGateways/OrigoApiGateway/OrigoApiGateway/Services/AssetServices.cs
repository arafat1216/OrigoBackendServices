using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace OrigoApiGateway.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, IHttpClientFactory httpClientFactory, IOptions<AssetConfiguration> options, IUserServices userServices, IUserPermissionService userPermissionService, IMapper mapper, IDepartmentsServices departmentsServices)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _userServices = userServices;
            _departmentsServices = departmentsServices;
            _mapper = mapper;
            _userPermissionService = userPermissionService;
        }
        private readonly IUserServices _userServices;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IDepartmentsServices _departmentsServices;
        private readonly ILogger<AssetServices> _logger;
        private readonly IMapper _mapper;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("assetservices");
        private readonly AssetConfiguration _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public async Task<PagedModel<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds, int page = 1, int limit = 25)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/customers/count/pagination?page={page}&limit={limit}", customerIds);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var assetCountList = await response.Content.ReadFromJsonAsync<PagedModel<CustomerAssetCount>>();
                return assetCountList;
            }
            catch (Exception exception)
            {
                throw new AssetException("GetAllCustomerAssetsCountAsync failed", exception);
            }
        }
        public async Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/customers/count", customerIds);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var assetCountList = await response.Content.ReadFromJsonAsync<IList<CustomerAssetCount>>();
                return assetCountList;
            }
            catch (Exception exception)
            {
                throw new AssetException("GetAllCustomerAssetsCountAsync failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("GetAssetsCountAsync failed", exception);
            }
        }

        public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
        {
            try
            {
                var totalBookValue = await HttpClient.GetFromJsonAsync<decimal>($"{_options.ApiPath}/customers/{customerId}/total-book-value");

                return totalBookValue;
            }
            catch (Exception exception)
            {
                throw new AssetException("GetCustomerTotalBookValue failed", exception);
            }
        }



        public async Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/users/{userId}?includeAsset={includeAsset}&includeImeis={includeImeis}&includeContractHolderUser={includeContractHolderUser}";

                var assets = await HttpClient.GetFromJsonAsync<IList<AssetDTO>>(requestUri);

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
            catch (Exception exception)
            {
                throw new AssetException("GetAssetsForUserAsync failed", exception);
            }
        }
        public async Task<PagedModel<HardwareSuperType>> GetAssetsForCustomerAsync(Guid customerId,
            CancellationToken cancellationToken, FilterOptionsForAsset filterOptions, string search = "", int page = 1,
            int limit = 25, bool includeAsset = false, bool includeImeis = false, bool includeLabels = false,
            bool includeContractHolderUser = false)
        {
            try
            {
                string json = JsonSerializer.Serialize(filterOptions);

                var requestUri = $"{_options.ApiPath}/customers/{customerId}?includeAsset={includeAsset}&includeImeis={includeImeis}&includeLabels={includeLabels}&includeContractHolderUser={includeContractHolderUser}&q={search}&page={page}&limit={limit}&filterOptions={json}";
                var assets = await HttpClient.GetFromJsonAsync<PagedModel<HardwareSuperType>>(requestUri, cancellationToken);
                if (assets == null) return new PagedModel<HardwareSuperType>();
                var users = await _userServices.GetAllUsersNamesAsync(customerId, cancellationToken);
                var departments = await _departmentsServices.GetAllDepartmentNamesAsync(customerId, cancellationToken);

                foreach (var asset in assets.Items)
                {
                    try
                    {
                        if (asset.ManagedByDepartmentId != null)
                        {
                            var departmentName = string.Empty;
                            if (departments.TryGetValue(new DepartmentNamesDTO { DepartmentId = asset.ManagedByDepartmentId.Value }, out var foundDepartmentNamesDTO))
                            {
                                departmentName = foundDepartmentNamesDTO.DepartmentName;
                            }
                            asset.DepartmentName = departmentName;
                        }

                        if (asset.AssetHolderId == null) continue;
                        var userName = string.Empty;
                        if(users.TryGetValue(new UserNamesDTO { UserId = asset.AssetHolderId.Value }, out var foundUserNamesDTO))
                        {
                            userName = foundUserNamesDTO.UserName;
                        }

                        asset.AssetHolderName = userName;
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
                throw new AssetException("SetLifeCycleSettingForCustomerAsync failed", exception);
            }
        }

        public async Task<DisposeSetting> GetDisposeSettingByCustomer(Guid customerId)
        {
            try
            {
                var settings = await HttpClient.GetFromJsonAsync<DisposeSetting>($"{_options.ApiPath}/customers/{customerId}/dispose-setting");

                if (settings == null)
                    return new DisposeSetting();

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
                throw new AssetException("SetDisposeSettingForCustomerAsync failed", exception);
            }
        }
        public async Task<IList<ReturnLocation>> GetReturnLocationsByCustomer(Guid customerId)
        {
            try
            {
                var settings = await HttpClient.GetFromJsonAsync<IList<ReturnLocation>>($"{_options.ApiPath}/customers/{customerId}/return-location");

                if (settings == null)
                    return null;

                return settings;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetReturnLocationsByCustomer failed with HttpRequestException.");
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetReturnLocationsByCustomer failed with content type is not valid.");
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetReturnLocationsByCustomer failed with invalid JSON.");
            }

            return null;
        }
        public async Task<ReturnLocation> AddReturnLocationsByCustomer(Guid customerId, NewReturnLocation data, IList<Location> officeLocation, Guid callerId)
        {
            try
            {
                if (!officeLocation.Select(x => x.Id).Contains(data.LocationId))
                {
                    throw new ResourceNotFoundException($"LocationId not found for this customer id: {customerId}", _logger);
                }

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/return-location";
                var newDTO = _mapper.Map<NewReturnLocationDTO>(data);
                newDTO.CallerId = callerId;
                var response = new HttpResponseMessage();
                response = await HttpClient.PostAsJsonAsync(requestUri, newDTO);


                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var newReturnLocation = await response.Content.ReadFromJsonAsync<ReturnLocation>();
                return newReturnLocation;
            }
            catch (Exception exception)
            {
                throw new AssetException("AddReturnLocationsByCustomer failed", exception);
            }
        }
        public async Task<ReturnLocation> UpdateReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, NewReturnLocation data, Guid callerId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/return-location/{returnLocationId}";
                var newDTO = _mapper.Map<NewReturnLocationDTO>(data);
                newDTO.CallerId = callerId;
                var response = new HttpResponseMessage();
                response = await HttpClient.PutAsJsonAsync(requestUri, newDTO);


                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var newReturnLocations = await response.Content.ReadFromJsonAsync<ReturnLocation>();
                return newReturnLocations;
            }
            catch (Exception exception)
            {
                throw new AssetException("UpdateReturnLocationsByCustomer failed", exception);
            }
        }
        public async Task<IList<ReturnLocation>> DeleteReturnLocationsByCustomer(Guid customerId, Guid returnLocationId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/customers/{customerId}/return-location/{returnLocationId}";
                var response = new HttpResponseMessage();
                response = await HttpClient.DeleteAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                    throw exception;
                }
                var newReturnLocations = await response.Content.ReadFromJsonAsync<IList<ReturnLocation>>();
                return newReturnLocations;
            }
            catch (Exception exception)
            {
                throw new AssetException("DeleteReturnLocationsByCustomer failed", exception);
            }
        }

        public async Task<AssetValidationResult> ImportAssetsFileAsync(Guid organizationId, IFormFile file, bool validateOnly, Organization organization)
        {
            var url = $"{_options.ApiPath}/customers/{organizationId}/import";
            var multiContent = new MultipartFormDataContent();
            multiContent.Add(new StreamContent(file.OpenReadStream()), "assetImportFile", file.FileName);
            var result = await HttpClient.PostAsync(url, multiContent);
            var assetValidationResults = await result.Content.ReadFromJsonAsync<AssetValidationResult>();
            if (assetValidationResults == null) return null;
            await CheckAndUpdateUserGivenEmail(organizationId, assetValidationResults);

            if (validateOnly)
            {
                return assetValidationResults;
            }
            var currency = CurrencyCode.NOK;
            switch (organization.Preferences.PrimaryLanguage)
            {
                case "no":
                    currency = CurrencyCode.NOK;
                    break;
                case "NO":
                    currency = CurrencyCode.NOK;
                    break;
                case "sv":
                    currency = CurrencyCode.SEK;
                    break;
                case "SV":
                    currency = CurrencyCode.SEK;
                    break;
                case "nb":
                    currency = CurrencyCode.NOK;
                    break;
                case "NB":
                    currency = CurrencyCode.NOK;
                    break;
                case "se":
                    currency = CurrencyCode.SEK;
                    break;
                case "SE":
                    currency = CurrencyCode.SEK;
                    break;
                case "dk":
                    currency= CurrencyCode.DKK;
                    break;
                case "DK":
                    currency = CurrencyCode.DKK;
                    break;
                default:
                    break;
            }


            var callerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            foreach (var validAsset in assetValidationResults.ValidAssets)
            {
                if (String.IsNullOrWhiteSpace(validAsset.ImportedUser.Email)) //need to handle assets that do not have an owner at all - asset should be input required
                {
                    //Set matched user id to null instead of to show that no one owns this asset
                    validAsset.MatchedUserId = null;

                    await AddAssetForCustomerAsync(organizationId,
                        new NewAssetDTO
                        {
                            Alias = validAsset.Alias,
                            AssetHolderId = validAsset.MatchedUserId, 
                            Brand = validAsset.Brand,
                            AssetCategoryId = 1,
                            Imei =
                                validAsset.Imeis.Split(",", StringSplitOptions.TrimEntries).Select(long.Parse).ToList(),
                            ProductName = validAsset.ProductName,
                            Source = "FileImport",
                            SerialNumber = validAsset.SerialNumber,
                            PurchaseDate = validAsset.PurchaseDate,
                            Labels = validAsset.Label != string.Empty ? new List<string> { validAsset.Label } : null,
                            PaidByCompany = new Common.Model.Money(decimal.Parse(validAsset.PurchasePrice), currency)
                        });

                    

                }
                else //the asset do have a user or need to import a new user 
                {

                    if (validAsset.MatchedUserId == Guid.Empty)
                    {
                        var newUser = await _userServices.AddUserForCustomerAsync(organizationId,
                            new NewUser
                            {
                                Email = validAsset.ImportedUser.Email,
                                FirstName = validAsset.ImportedUser.FirstName,
                                LastName = validAsset.ImportedUser.LastName,
                                MobileNumber = validAsset.ImportedUser.PhoneNumber
                            }, callerId, true);
                        validAsset.MatchedUserId = newUser.Id;
                    }

                    await AddAssetForCustomerAsync(organizationId,
                        new NewAssetDTO
                        {
                            Alias = validAsset.Alias,
                            AssetHolderId = validAsset.MatchedUserId,
                            Brand = validAsset.Brand,
                            AssetCategoryId = 1,
                            Imei =
                                validAsset.Imeis.Split(",", StringSplitOptions.TrimEntries).Select(long.Parse).ToList(),
                            ProductName = validAsset.ProductName,
                            Source = "FileImport",
                            SerialNumber = validAsset.SerialNumber,
                            PurchaseDate = validAsset.PurchaseDate,
                            PurchasedBy = validAsset.ImportedUser.FirstName + ' ' + validAsset.ImportedUser.LastName,
                            Labels = validAsset.Label != string.Empty ? new List<string> { validAsset.Label } : null,
                            PaidByCompany = new Common.Model.Money(decimal.Parse(validAsset.PurchasePrice), currency)
                        });
                }
            }

            return assetValidationResults;
        }

        /// <summary>
        /// Checks if email addresses exists. If it does and the organization is not the correct one it is marked and
        /// moved to the incorrect assets.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="assetValidationResults"></param>
        /// <returns></returns>
        private async Task CheckAndUpdateUserGivenEmail(Guid organizationId, AssetValidationResult assetValidationResults)
        {
            foreach (var validAsset in assetValidationResults.ValidAssets)
            {
                var invalidAssetErrors = new List<string>();
                //Validate email - if no username no user
                if (!String.IsNullOrWhiteSpace(validAsset.ImportedUser.Email))
                {
                    var user = await _userServices.UserEmailLinkedToGivenOrganization(organizationId,
                        validAsset.ImportedUser.Email);
                    if (user.correctOrganization)
                    {
                        validAsset.MatchedUserId = user.userId;
                    }
                    else
                    {
                        invalidAssetErrors.Add("Email is not unique and is used outside of the organization.");
                    }

                    //Validate phonenumber
                    if (!String.IsNullOrWhiteSpace(validAsset.ImportedUser.PhoneNumber))
                    {
                        var mobileNumberAssignedToUser = await _userServices.GetUserWithPhoneNumber(organizationId, validAsset.ImportedUser.PhoneNumber);
                        if (mobileNumberAssignedToUser != null)
                        {
                            //If the user is not the same user 
                            if (mobileNumberAssignedToUser.UserName != validAsset.ImportedUser.Email)
                            {
                                invalidAssetErrors.Add("Phone number is not unique and is used in the organization.");
                                validAsset.MatchedUserId = null;
                            }
                        }
                    }

                    if (invalidAssetErrors.Any())
                    {
                        var invalidAsset = _mapper.Map<InvalidImportedAsset>(validAsset);
                        invalidAsset.Errors = invalidAssetErrors;
                        assetValidationResults.InvalidAssets.Add(invalidAsset);
                    }

                }
                else validAsset.MatchedUserId = Guid.Empty; // need to import user

            }
            
            assetValidationResults.ValidAssets.RemoveAll(a => a.MatchedUserId == null);
        }

        public async Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId, FilterOptionsForAsset? filterOptions,
            bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false)
        {
            try
            {

                var requestUri = $"{_options.ApiPath}/{assetId}/customers/{customerId}?includeAsset={includeAsset}&includeImeis={includeImeis}&includeLabels={includeLabels}&includeContractHolderUser={includeContractHolderUser}";

                if (filterOptions != null)
                {
                    string json = JsonSerializer.Serialize(filterOptions);
                    requestUri = QueryHelpers.AddQueryString(requestUri, "filterOptions", $"{json}");
                }

                var asset = await HttpClient.GetFromJsonAsync<AssetDTO>(requestUri);

                OrigoAsset result = null;
                if (asset != null)
                {
                    if (asset.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(asset);
                    else
                        result = _mapper.Map<OrigoTablet>(asset);

                    try
                    {
                        if (asset.ManagedByDepartmentId != null)
                        {
                            var department = await _departmentsServices.GetDepartmentAsync(asset.OrganizationId, asset.ManagedByDepartmentId ?? throw new ArgumentNullException("DepartmentId"));
                            if (department != null) result.DepartmentName = department.Name;
                        }
                        if (asset.AssetHolderId != null)
                        {
                            var user = await _userServices.GetUserAsync(asset.AssetHolderId ?? throw new ArgumentNullException("UserId"));
                            if (user != null) result.AssetHolderName = user.DisplayName;
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

                    try
                    {
                        if (asset.ManagedByDepartmentId != null)
                        {
                            var department = await _departmentsServices.GetDepartmentAsync(asset.OrganizationId, asset.ManagedByDepartmentId ?? throw new ArgumentNullException("DepartmentId"));
                            if (department != null) result.DepartmentName = department.Name;
                        }
                        if (asset.AssetHolderId != null)
                        {
                            var user = await _userServices.GetUserAsync(asset.AssetHolderId ?? throw new ArgumentNullException("UserId"));
                            if (user != null) result.AssetHolderName = user.DisplayName;
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

                return result;
            }
            catch (Exception exception)
            {
                throw new AssetException("AddAssetForCustomerAsync failed", exception);
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
                throw new AssetException("UpdateStatusOnAssets failed", exception);
            }
        }
        public async Task<OrigoAsset> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailable data, Guid callerId)
        {
            try
            {

                var makeAssetAvailableDTO = _mapper.Map<MakeAssetAvailableDTO>(data);
                makeAssetAvailableDTO.CallerId = callerId; // Guid.Empty if tryparse fails.

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/make-available";
                var existingAsset = await GetAssetForCustomerAsync(customerId, data.AssetLifeCycleId, null);
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
                throw new AssetException("MakeAssetAvailableAsync failed", exception);
            }
        }
        public async Task<OrigoAsset> ReturnDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, Guid returnLocationId, Guid callerId)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetLifeCycleId, null);
                if (existingAsset == null) throw new ResourceNotFoundException("Asset Not Found!!", _logger);
                if((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && !accessList.Contains(existingAsset.ManagedByDepartmentId))
                    throw new UnauthorizedAccessException("Manager does not have access to this asset!!!");

                var returnDTO = new ReturnDeviceDTO()
                {
                    AssetLifeCycleId = assetLifeCycleId,
                    CallerId = callerId,
                    ReturnLocationId = returnLocationId,
                    Role = role
                };
                if (role == PredefinedRole.EndUser.ToString() && existingAsset.IsPersonal && existingAsset.AssetStatus == AssetLifecycleStatus.PendingReturn)
                {
                    throw new AssetException("Return Request Already Pending!!!");
                }
                else if(role == PredefinedRole.EndUser.ToString() && existingAsset.IsPersonal && existingAsset.AssetStatus != AssetLifecycleStatus.PendingReturn)
                {
                    if (existingAsset.AssetHolderId != callerId && role == PredefinedRole.EndUser.ToString()) throw new Exception("Only ContractHolderUser can make Return Request!!!");
                    if(existingAsset.ManagedByDepartmentId != null)
                    {
                        var department = await _departmentsServices.GetDepartmentAsync(customerId, existingAsset.ManagedByDepartmentId.Value);
                        var managers = new List<EmailPersonAttributeDTO>();
                        foreach (var deptManager in department.ManagedBy)
                        {
                            var manager = await _userServices.GetUserAsync(customerId, deptManager.UserId);
                            managers.Add(new EmailPersonAttributeDTO()
                            {
                                Name = manager.FirstName,
                                Email = manager.Email,
                                PreferedLanguage = manager.UserPreference!.Language
                            });
                        }
                        returnDTO.Managers = managers;
                    }
                }
                else if (!existingAsset.IsPersonal)
                {
                    if (role == PredefinedRole.EndUser.ToString()) throw new Exception("Only Admin can make return request for non-personal asset!!!");
                }
                else if (existingAsset.IsPersonal && role != PredefinedRole.EndUser.ToString())
                {
                    if (returnLocationId == Guid.Empty) throw new Exception("Must Select a Return Location to Confirm!!!");
                    var user = await _userServices.GetUserAsync(customerId, existingAsset.AssetHolderId!.Value);
                    returnDTO.User = new EmailPersonAttributeDTO()
                    {
                        Name = user.FirstName,
                        Email = user.Email,
                        PreferedLanguage = user.UserPreference!.Language
                    };
                }
                else
                {
                    throw new AssetException("Asset not allowed to Return!!!");
                }

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/return-device";

                var response = await HttpClient.PostAsJsonAsync(requestUri, returnDTO);
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
            catch(ResourceNotFoundException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AssetException("ReturnDeviceAsync failed", exception);
            }
        }
        public async Task<OrigoAsset> BuyoutDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, string payrollContactEmail, Guid callerId)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetLifeCycleId, null);
                if (existingAsset == null) throw new ResourceNotFoundException("Asset Not Found!!", _logger);
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && !accessList.Contains(existingAsset.ManagedByDepartmentId))
                    throw new UnauthorizedAccessException("Manager does not have access to this asset!!!");

                if (existingAsset.AssetHolderId != callerId && role == PredefinedRole.EndUser.ToString()) throw new Exception("Only ContractHolderUser can do buyout!!!");
                
                var buyoutDTO = new BuyoutDeviceDTO()
                {
                    AssetLifeCycleId = assetLifeCycleId,
                    PayrollContactEmail = payrollContactEmail,
                    CallerId = callerId
                };

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/buyout-device";

                var response = await HttpClient.PostAsJsonAsync(requestUri, buyoutDTO);
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
            catch(ResourceNotFoundException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AssetException("BuyoutDeviceAsync failed", exception);
            }
        }
        public async Task<OrigoAsset> PendingBuyoutDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, string currency, Guid callerId)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetLifeCycleId, null);
                if (existingAsset == null) throw new ResourceNotFoundException("Asset Not Found!!", _logger);
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && !accessList.Contains(existingAsset.ManagedByDepartmentId))
                    throw new UnauthorizedAccessException("Manager does not have access to this asset!!!");

                if (existingAsset.AssetHolderId != callerId && role == PredefinedRole.EndUser.ToString()) throw new Exception("Only ContractHolderUser can do buyout!!!");

                var buyoutDTO = new PendingBuyoutDeviceDTO()
                {
                    AssetLifeCycleId = assetLifeCycleId,
                    CallerId = callerId,
                    Role = role,
                    Currency = currency,
                };

                if(existingAsset.AssetHolderId == null)
                {
                    throw new PendingBuyoutException("No Asset Contract Holder found!!!", _logger);
                }
                else
                {
                    var user = await _userServices.GetUserAsync(existingAsset.AssetHolderId.Value);
                    if(user.UserStatus != (int)UserStatus.OffboardInitiated && user.UserStatus != (int)UserStatus.OffboardOverdue)
                        throw new PendingBuyoutException("User is not Offboarding!!!", _logger);

                    buyoutDTO.User = new EmailPersonAttributeDTO()
                    {
                        Email = user.Email,
                        Name = user.FirstName,
                        PreferedLanguage = user.UserPreference.Language
                    };
                    if (user.LastWorkingDay == null)
                        throw new PendingBuyoutException("User does not have LastWorkingDay set!!!", _logger);

                    buyoutDTO.LasWorkingDay = user.LastWorkingDay.Value;
                }

                if(role != PredefinedRole.EndUser.ToString() && callerId != Guid.Empty)
                {
                    var manager = await _userServices.GetUserAsync(callerId);
                    if (manager != null)
                    {
                        buyoutDTO.ManagerName = $"{manager.FirstName} {manager.LastName}";
                    }
                }

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/pending-buyout";

                var response = await HttpClient.PostAsJsonAsync(requestUri, buyoutDTO);
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
                throw new AssetException("PendingBuyoutDeviceAsync failed", exception);
            }
        }

        public async Task<OrigoAsset> ReportDeviceAsync(Guid customerId, ReportDevice data, string role, List<Guid?> accessList, Guid callerId)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, data.AssetId, null, includeContractHolderUser: true);
                if (existingAsset == null) throw new ResourceNotFoundException("Asset Not Found!!", _logger);
                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && !accessList.Contains(existingAsset.ManagedByDepartmentId))
                    throw new UnauthorizedAccessException("Manager does not have access to this asset!!!");

                if (existingAsset.IsPersonal)
                {
                    if (existingAsset.AssetHolderId != callerId && role == PredefinedRole.EndUser.ToString())
                    {
                        throw new AssetException("Only ContractHolderUser can Report!!!");
                    }
                }

                var reportDTO = new ReportDeviceDTO()
                {
                    AssetLifeCycleId = data.AssetId,
                    CallerId = callerId,
                    ReportCategory = data.ReportCategory,
                    Description = data.Description,
                    TimePeriodFrom = data.TimePeriodFrom,
                    TimePeriodTo = data.TimePeriodTo,
                    City = data.City,
                    Country = data.Country,
                    Address =data.Address,
                    PostalCode = data.PostalCode
                };

                if (!existingAsset.IsPersonal)
                {
                    var customerAdmin = await _userPermissionService.GetAllCustomerAdminsAsync(customerId);
                    var admins = new List<EmailPersonAttributeDTO>();
                    foreach (var admin in customerAdmin)
                    {
                        admins.Add(new EmailPersonAttributeDTO()
                        {
                            Name = admin.FirstName,
                            Email = admin.Email
                        });
                    }
                    reportDTO.CustomerAdmins = admins;
                }
                OrigoUser user = null;
                if (existingAsset.AssetHolderId != null)
                {
                    user = await _userServices.GetUserAsync(existingAsset.AssetHolderId.Value);
                    reportDTO.ContractHolderUser = new EmailPersonAttributeDTO()
                    {
                        Email = user.Email,
                        Name = user.FirstName,
                        PreferedLanguage = user.UserPreference.Language
                    };
                }

                if (callerId != Guid.Empty)
                {
                    if (callerId == existingAsset.AssetHolderId)
                    {
                        reportDTO.ReportedBy = $"{user!.DisplayName} - {user.Email}";
                    }
                    else
                    {
                        var reportedUser = await _userServices.GetUserAsync(callerId);
                        reportDTO.ReportedBy = $"{reportedUser.DisplayName} - {reportedUser.Email}";
                    }
                }

                if (existingAsset.ManagedByDepartmentId != null)
                {
                    var department = await _departmentsServices.GetDepartmentAsync(customerId, existingAsset.ManagedByDepartmentId.Value);
                    var managers = new List<EmailPersonAttributeDTO>();
                    foreach (var deptManager in department.ManagedBy)
                    {
                        var manager = await _userServices.GetUserAsync(customerId, deptManager.UserId);
                        managers.Add(new EmailPersonAttributeDTO()
                        {
                            Name = manager.FirstName,
                            Email = manager.Email,
                            PreferedLanguage = manager.UserPreference!.Language
                        });
                    }
                    reportDTO.Managers = managers;
                }

                var requestUri = $"{_options.ApiPath}/customers/{customerId}/report-device";

                var response = await HttpClient.PostAsJsonAsync(requestUri, reportDTO);
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
            catch(ResourceNotFoundException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AssetException("ReportDeviceAsync failed", exception);
            }
        }

        public async Task<OrigoAsset> ReAssignAssetToDepartment(Guid customerId, Guid assetId, ReassignedToDepartmentDTO data)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetId, null);
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
                throw new AssetException("ReAssignAssetToDepartment failed", exception);
            }
        }

        public async Task<OrigoAsset> ReAssignAssetToUser(Guid customerId, Guid assetId, ReassignedToUserDTO data)
        {
            try
            {
                var existingAsset = await GetAssetForCustomerAsync(customerId, assetId, null);
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
                throw new AssetException("ReAssignAssetToUser failed", exception);
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
                throw new AssetException("UpdateAssetAsync failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("CreateLabelsForCustomerAsync failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("GetCustomerLabelsAsync failed", exception);
            }
        }

        public async Task<IList<Label>> DeleteCustomerLabelsAsync(Guid customerId, DeleteCustomerLabelsData data)
        {
            try
            {
                var requestUri = $"{HttpClient.BaseAddress}{_options.ApiPath}/customers/{customerId}/labels";
                var deleteRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri),
                    Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                };
                var response = await HttpClient.SendAsync(deleteRequestMessage);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
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
            catch (Exception exception)
            {
                throw new AssetException("DeleteCustomerLabelsAsync failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("UpdateLabelsForCustomerAsync failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("AssignLabelsToAssets failed", exception);
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
            catch (Exception exception)
            {
                throw new AssetException("UnAssignLabelsFromAssets failed", exception);
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
            if (string.IsNullOrEmpty(country)) return CurrencyCode.NOK.ToString();

            return country.ToUpper().Trim() switch
            {
                "NO" => CurrencyCode.NOK.ToString(),
                "SE" => CurrencyCode.SEK.ToString(),
                "DK" => CurrencyCode.DKK.ToString(),
                "US" => CurrencyCode.USD.ToString(),
                _ => CurrencyCode.EUR.ToString()
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
                throw new AssetException("ChangeLifecycleType failed", exception);
            }
        }

        public async Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, AssignAssetToUser assignedAsset)
        {
            try
            {
                var mappedAsset = _mapper.Map<AssignAssetToUserDTO>(assignedAsset);
                OrigoUser user = null;
                OrigoDepartment department = null;
                if (mappedAsset.UserId != Guid.Empty)
                {
                    try
                    {
                        user = await _userServices.GetUserAsync(customerId, mappedAsset.UserId);
                        if (user == null)
                            throw new BadHttpRequestException("Unable to assign asset. User not found");
                        mappedAsset.UserAssigneToDepartment = user.AssignedToDepartment;

                    }
                    catch
                    {
                        var exception = new BadHttpRequestException("Unable to assign asset. User not found");
                        _logger.LogError(exception, "Unable to assign asset. User not found");
                        throw exception;
                    }
                }
                if (mappedAsset.DepartmentId != Guid.Empty)
                {
                    try
                    {
                        department = await _departmentsServices.GetDepartmentAsync(customerId, mappedAsset.DepartmentId);
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

                var requestUri = $"{_options.ApiPath}/{mappedAsset.AssetId}/customer/{customerId}/assign";
                var response = await HttpClient.PostAsJsonAsync(requestUri, mappedAsset);
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

                    if (asset.ManagedByDepartmentId != null)
                    {
                        if(department == null)
                            department = await _departmentsServices.GetDepartmentAsync(asset.OrganizationId, asset.ManagedByDepartmentId ?? throw new ArgumentNullException("DepartmentId"));
                        result.DepartmentName = department.Name;
                    }

                    if (asset.AssetHolderId != null)
                    {
                        if (user == null)
                            user = await _userServices.GetUserAsync(customerId, asset.AssetHolderId ?? throw new ArgumentNullException("UserId"));
                        result.AssetHolderName = user.DisplayName;
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                throw new AssetException("AssignAsset failed", exception);
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
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "purchaseDate", Required = true });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "purchasedBy", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "managedByDepartmentId", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "assetHolderId", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "createdDate", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "startPeriod", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "endPeriod", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "paidByCompany", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "bookValue", Required = false });
            defaultAttributes.Add(new AssetCategoryAttribute { Name = "offboardBuyoutPrice", Required = false });
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
            catch (Exception exception)
            {
                throw new AssetException("GetAssetAuditLog failed", exception);
            }
        }
        public async Task<string> CreateAssetSeedData()
        {
            try
            {
                var errorMessage = await HttpClient.GetStringAsync($"{_options.ApiPath}/seed");
                return errorMessage;
            }
            catch (Exception exception)
            {
                throw new AssetException("CreateAssetSeedData failed", exception);
            }
        }

        public async Task<OrigoCustomerAssetsCounter> GetAssetLifecycleCountersAsync(Guid customerId, FilterOptionsForAsset filter)
        {

            try
            {

                string json = JsonSerializer.Serialize(filter);
                return await HttpClient.GetFromJsonAsync<OrigoCustomerAssetsCounter>($"{_options.ApiPath}/customers/{customerId}/assets-counter/?filter={json}");


            }
            catch (Exception exception)
            {
                throw new AssetException("GetAssetLifecycleCountersAsync failed", exception);
            }

        }

        public async Task<IList<OrigoAsset>> ActivateAssetStatusOnAssetLifecycle(Guid customerId, ChangeAssetStatusDTO changedAssetStatus)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/customers/{customerId}/activate", changedAssetStatus);
                
                
                var assetLifecycles = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();
                if (assetLifecycles == null)
                {
                    return null;
                }
                
                IList<OrigoAsset> assets = new List<OrigoAsset>();
                OrigoAsset result = null;
                foreach (var assetLifecycle in assetLifecycles) 
                { 
                if (assetLifecycle != null)
                {
                    if (assetLifecycle.AssetCategoryId == 1)
                        result = _mapper.Map<OrigoMobilePhone>(assetLifecycle);
                    else
                        result = _mapper.Map<OrigoTablet>(assetLifecycle);
                        assets.Add(result);
                }
            }

                    return assets;
            }
            catch (Exception exception)
            {
                throw new AssetException("ActivateAssetStatusOnAssetLifecycle failed", exception);
            }
        }

        public async Task<IList<HardwareSuperType>> DeactivateAssetStatusOnAssetLifecycle(Guid customerId, ChangeAssetStatusDTO changedAssetStatus)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/customers/{customerId}/deactivate", changedAssetStatus);


                var assetLifecycles = await response.Content.ReadFromJsonAsync<IList<AssetDTO>>();

                if (assetLifecycles == null)
                {
                    return null;
                }

                return _mapper.Map<IList<HardwareSuperType>>(assetLifecycles);
            }
            catch (Exception exception)
            {
                throw new AssetException("DeactivateAssetStatusOnAssetLifecycle failed", exception);
            }
        }
    }
}