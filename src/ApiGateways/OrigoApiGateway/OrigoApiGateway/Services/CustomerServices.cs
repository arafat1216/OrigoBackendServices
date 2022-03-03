using AutoMapper;
using Common.Enums;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class CustomerServices : ICustomerServices
    {
        public CustomerServices(ILogger<CustomerServices> logger, HttpClient httpClient,
            IOptions<CustomerConfiguration> options, IAssetServices assetServices, IMapper mapper)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _assetServices = assetServices;
            _mapper = mapper;
        }

        private readonly ILogger<CustomerServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly CustomerConfiguration _options;
        private readonly IAssetServices _assetServices;
        private readonly IMapper _mapper;

        public async Task<IList<Organization>> GetCustomersAsync()
        {
            try
            {
                bool customersOnly = true;
                var organizations = await HttpClient.GetFromJsonAsync<IList<OrganizationDTO>>($"{_options.ApiPath}/{customersOnly}");
                if (organizations == null)
                    return null;

                return _mapper.Map<List<Organization>>(organizations);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetCustomersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetCustomersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomersAsync unknown error.");
                throw;
            }
        }

        public async Task<Organization> GetCustomerAsync(Guid customerId)
        {
            try
            {
                bool customerOnly = true;
                var organization = await HttpClient.GetFromJsonAsync<OrganizationDTO>($"{_options.ApiPath}/{customerId}/{customerOnly}");
                return organization == null ? null : _mapper.Map<Organization>(organization);
            }
            catch (HttpRequestException exception)
            {
                // Not found
                if ((int)exception.StatusCode == 404)
                {
                    return null;
                }

                _logger.LogError(exception, "GetCustomerAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetCustomerAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<IList<CustomerUserCount>> GetCustomerUsersAsync()
        {
            try
            {
                var customerUserCounts = await HttpClient.GetFromJsonAsync<IList<CustomerUserCount>>($"{_options.ApiPath}/userCount");
                return customerUserCounts == null ? null : _mapper.Map<IList<CustomerUserCount>>(customerUserCounts);
            }
            catch (HttpRequestException exception)
            {
                // Not found
                if ((int)exception.StatusCode == 404)
                {
                    return null;
                }

                _logger.LogError(exception, "GetCustomerUsersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetCustomerUsersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomerUsersAsync unknown error.");
                throw;
            }
        }

        public async Task<Organization> CreateCustomerAsync(NewOrganization newCustomer, Guid callerId)
        {
            try
            {
                var newCustomerDTO = _mapper.Map<NewOrganizationDTO>(newCustomer);
                newCustomerDTO.CallerId = callerId;
                newCustomerDTO.IsCustomer = true;

                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomerDTO);
                if (!response.IsSuccessStatusCode && (int)response.StatusCode == 409)
                    throw new InvalidOrganizationNumberException(await response.Content.ReadAsStringAsync());
                else if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save customer", (int)response.StatusCode);

                var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
                return organization == null ? null : _mapper.Map<Organization>(organization);
            }
            catch (InvalidOrganizationNumberException exception)
            {
                _logger.LogError(exception, "CreateCustomerAsync failed with InvalidOrganizationNumberException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId)
        {
            try
            {
                DeleteOrganization delOrg = new DeleteOrganization
                {
                    OrganizationId = organizationId,
                    CallerId = callerId,
                    hardDelete = false
                };

                var requestUri = $"{_options.ApiPath}/{organizationId}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(delOrg),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if ((int)response.StatusCode == 404)
                        return null;
                    var exception = new BadHttpRequestException("Unable to remove organization.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove organization.");
                    throw exception;
                }

                var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
                return organization == null ? null : _mapper.Map<Organization>(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
                throw;
            }
        }

        public async Task<Organization> UpdateOrganizationAsync(UpdateOrganizationDTO organizationToChange)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{organizationToChange.OrganizationId}/organization", organizationToChange);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to update organization", (int)response.StatusCode);

                var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
                return organization == null ? null : _mapper.Map<Organization>(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
                throw;
            }
        }

        public async Task<Organization> PatchOrganizationAsync(UpdateOrganizationDTO organizationToChange)
        {
            try
            {
                var response = await HttpClient.PostAsync($"{_options.ApiPath}/{organizationToChange.OrganizationId}/organization", JsonContent.Create(organizationToChange));
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to update organization", (int)response.StatusCode);

                var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
                return organization == null ? null : _mapper.Map<Organization>(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
                throw;
            }
        }

        public async Task<IList<OrigoCustomerAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId)
        {
            try
            {
                var categories = await HttpClient.GetFromJsonAsync<IList<CustomerAssetCategoryDTO>>($"{_options.ApiPath}/{customerId}/assetCategory");
                if (categories == null) return null;
                var assetCategories = await _assetServices.GetAssetCategoriesAsync();
                var assetLifecycles = await _assetServices.GetLifecycles();
                var origoAssetCategories = new List<OrigoCustomerAssetCategoryType>();
                origoAssetCategories.AddRange(assetCategories.Select(category => new OrigoCustomerAssetCategoryType
                {
                    Name = category.Name,
                    AssetCategoryId = category.AssetCategoryId,
                    IsChecked = false,
                    LifecycleTypes = assetLifecycles.Select(l => new OrigoAssetCategoryLifecycleType()
                    {
                        AssetCategoryId = category.AssetCategoryId,
                        LifecycleType = (LifecycleType)l.EnumValue,
                        Name = l.Name,
                        IsChecked = false
                    }).ToList()
                }));
                foreach (var category in origoAssetCategories)
                {
                    var customerCategory = categories.FirstOrDefault(c => c.AssetCategoryId == category.AssetCategoryId);
                    if (customerCategory != null)
                    {
                        category.IsChecked = true;

                        foreach (var lifecycle in category.LifecycleTypes)
                        {
                            var customerLifecycle = customerCategory.LifecycleTypes.FirstOrDefault(c => c.AssetCategoryId == category.AssetCategoryId && c.LifecycleType == lifecycle.LifecycleType);
                            if (customerLifecycle != null)
                            {
                                lifecycle.IsChecked = true;
                            }
                        }
                    }
                }

                return origoAssetCategories;
            }
            catch (HttpRequestException exception)
            {
                // Customer not found
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "GetAssetCategoryForCustomerAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetCategoryForCustomerAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetCategoryForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoCustomerAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType, Guid callerId)
        {
            try
            {
                var customerAssetCategoryTypeDTO = _mapper.Map<NewCustomerAssetCategoryTypeDTO>(customerAssetCategoryType);
                customerAssetCategoryTypeDTO.CallerId = callerId;

                var assetCategories = await _assetServices.GetAssetCategoriesAsync();
                var assetCategory = assetCategories.FirstOrDefault(a => a.AssetCategoryId == customerAssetCategoryTypeDTO.AssetCategoryId);
                if (assetCategory == null)
                    return null;
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory";
                var response = await HttpClient.PatchAsync(requestUri, JsonContent.Create(customerAssetCategoryTypeDTO));
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to add the asset category to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to add the asset category to the customer.");
                    throw exception;
                }
                var assetLifecycles = await _assetServices.GetLifecycles();
                var category = await response.Content.ReadFromJsonAsync<CustomerAssetCategoryDTO>();
                var tempAssetCategory = category == null ? null : new OrigoCustomerAssetCategoryType
                {
                    AssetCategoryId = category.AssetCategoryId,
                    Name = assetCategory?.Name,
                    IsChecked = true,
                    LifecycleTypes = assetLifecycles.Select(l => new OrigoAssetCategoryLifecycleType()
                    {
                        AssetCategoryId = category.AssetCategoryId,
                        LifecycleType = (LifecycleType)l.EnumValue,
                        Name = l.Name,
                    }).ToList()
                };
                if (tempAssetCategory != null)
                {
                    foreach (var lifecycle in tempAssetCategory.LifecycleTypes)
                    {
                        var customerLifecycle = category.LifecycleTypes.FirstOrDefault(c => c.LifecycleType == lifecycle.LifecycleType);
                        if (customerLifecycle != null)
                        {
                            lifecycle.IsChecked = true;
                        }
                    }
                }
                return tempAssetCategory;
            }
            catch (InvalidLifecycleTypeException exception)
            {
                _logger.LogError(exception, "AddAssetCategoryForCustomerAsync invalid lifecycletype");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddAssetCategoryForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoCustomerAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType, Guid callerId)
        {
            try
            {
                var customerAssetCategoryTypeDTO = _mapper.Map<NewCustomerAssetCategoryTypeDTO>(customerAssetCategoryType);
                customerAssetCategoryTypeDTO.CallerId = callerId;
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory";
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(customerAssetCategoryTypeDTO),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };
                var response = await HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove the asset category to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove the asset category to the customer.");
                    throw exception;
                }
                var assetCategories = await _assetServices.GetAssetCategoriesAsync();
                var assetCategory = assetCategories.FirstOrDefault(a => a.AssetCategoryId == customerAssetCategoryTypeDTO.AssetCategoryId);
                var assetLifecycles = await _assetServices.GetLifecycles();
                var category = await response.Content.ReadFromJsonAsync<CustomerAssetCategoryDTO>();
                var tempAssetCategory = category == null ? null : new OrigoCustomerAssetCategoryType
                {
                    AssetCategoryId = category.AssetCategoryId,
                    Name = assetCategory?.Name,
                    IsChecked = customerAssetCategoryTypeDTO.LifecycleTypes.Any(),
                    LifecycleTypes = assetLifecycles.Select(l => new OrigoAssetCategoryLifecycleType()
                    {
                        AssetCategoryId = category.AssetCategoryId,
                        LifecycleType = (LifecycleType)l.EnumValue,
                        Name = l.Name,
                        IsChecked = false
                    }).ToList()
                };
                if (tempAssetCategory != null)
                {
                    foreach (var lifecycle in tempAssetCategory.LifecycleTypes)
                    {
                        var customerLifecycle = category.LifecycleTypes.FirstOrDefault(c => c.LifecycleType == lifecycle.LifecycleType);
                        if (customerLifecycle != null)
                        {
                            lifecycle.IsChecked = true;
                        }
                    }
                }
                return tempAssetCategory;
            }
            catch (InvalidLifecycleTypeException exception)
            {
                _logger.LogError(exception, "RemoveAssetCategoryForCustomerAsync invalid lifecycletype");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RemoveAssetCategoryForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<string> CreateOrganizationSeedData()
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

        public async Task<string> GetOktaUserProfileByEmail(string email)
        {
            try
            {
                var result = await HttpClient.GetStringAsync($"{_options.ApiPath}/{email}/webshopUser");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOktaUserProfileByEmail error.");
                throw;
            }
        }

        public Task<bool> CheckAndProvisionWebShopUser(string email, string orgnumber)
        {
            throw new NotImplementedException();
        }

        //public async Task<string> CheckAndProvisionWebShopUser(string email, string orgnumber); //$"{_options.ApiPath}/webshopUser
    }
}