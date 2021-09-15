using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Enums;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class CustomerServices : ICustomerServices
    {
        public CustomerServices(ILogger<CustomerServices> logger, HttpClient httpClient,
            IOptions<CustomerConfiguration> options, IAssetServices assetServices)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _assetServices = assetServices;
        }

        private readonly ILogger<CustomerServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly CustomerConfiguration _options;
        private readonly IAssetServices _assetServices;

        public async Task<IList<Organization>> GetCustomersAsync()
        {
            try
            {
                var customers = await HttpClient.GetFromJsonAsync<IList<OrganizationDTO>>($"{_options.ApiPath}");
                if (customers == null) return null;
                var origoCustomers = new List<Organization>();
                foreach (var customer in customers) origoCustomers.Add(new Organization(customer));
                return origoCustomers;
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
                var customer = await HttpClient.GetFromJsonAsync<OrganizationDTO>($"{_options.ApiPath}/{customerId}");
                return customer == null ? null : new Organization(customer);
            }
            catch (HttpRequestException exception)
            {
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

        public async Task<Organization> CreateCustomerAsync(NewOrganization newCustomer)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomer);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save customer", (int)response.StatusCode);

                var customer = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
                return customer == null ? null : new Organization(customer);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateCustomerAsync unknown error.");
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

        public async Task<OrigoCustomerAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                var assetCategories = await _assetServices.GetAssetCategoriesAsync();
                var assetCategory = assetCategories.FirstOrDefault(a => a.AssetCategoryId == customerAssetCategoryType.AssetCategoryId);
                if (assetCategory == null)
                    return null;
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory";
                var response = await HttpClient.PatchAsync(requestUri, JsonContent.Create(customerAssetCategoryType));
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

        public async Task<OrigoCustomerAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory";
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(customerAssetCategoryType),
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
                var assetCategory = assetCategories.FirstOrDefault(a => a.AssetCategoryId == customerAssetCategoryType.AssetCategoryId);
                var assetLifecycles = await _assetServices.GetLifecycles();
                var category = await response.Content.ReadFromJsonAsync<CustomerAssetCategoryDTO>();
                var tempAssetCategory = category == null ? null : new OrigoCustomerAssetCategoryType
                {
                    AssetCategoryId = category.AssetCategoryId,
                    Name = assetCategory?.Name,
                    IsChecked = customerAssetCategoryType.LifecycleTypes.Any(),
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

        public async Task<IList<OrigoProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            try
            {
                var customerModules = await HttpClient.GetFromJsonAsync<IList<ModuleDTO>>($"{_options.ApiPath}/{customerId}/modules");
                var customerModulesList = new List<OrigoProductModule>();

                if (customerModules == null) return null;
                customerModulesList.AddRange(customerModules.Select(module => new OrigoProductModule()
                {
                    ProductModuleId = module.ProductModuleId,
                    Name = module.Name,
                    IsChecked = true,
                    ProductModuleGroup = module.ProductModuleGroup.Select(s => new OrigoProductModuleGroup(s) { IsChecked = true }).ToList()
                }));
                return customerModulesList;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetCustomerProductModulesAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetCustomerProductModulesAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomerProductModulesAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoProductModule> AddProductModulesAsync(Guid customerId, NewCustomerProductModule productModule)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/modules";
                var response = await HttpClient.PatchAsync(requestUri, JsonContent.Create(productModule));
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to add the module to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to add the module to the customer.");
                    throw exception;
                }
                var customerModules = await response.Content.ReadFromJsonAsync<ModuleDTO>();
                return customerModules == null ? null : new OrigoProductModule()
                {
                    ProductModuleId = customerModules.ProductModuleId,
                    Name = customerModules.Name,
                    IsChecked = true,
                    ProductModuleGroup = customerModules.ProductModuleGroup.Select(s => new OrigoProductModuleGroup(s) { IsChecked = true }).ToList()
                };
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "AddProductModulesAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "AddProductModulesAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddProductModulesAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoProductModule> RemoveProductModulesAsync(Guid customerId, NewCustomerProductModule productModule)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/modules";
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(productModule),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };
                var response = await HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove the module  to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove the module to the customer.");
                    throw exception;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return null;
                var customerModules = await response.Content.ReadFromJsonAsync<ModuleDTO>();
                return customerModules == null ? null : new OrigoProductModule()
                {
                    ProductModuleId = customerModules.ProductModuleId,
                    Name = customerModules.Name,
                    IsChecked = true,
                    ProductModuleGroup = customerModules.ProductModuleGroup.Select(s => new OrigoProductModuleGroup(s) { IsChecked = true }).ToList()
                };
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "RemoveProductModulesAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "RemoveProductModulesAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RemoveProductModulesAsync unknown error.");
                throw;
            }
        }
    }
}