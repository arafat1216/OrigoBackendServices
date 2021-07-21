﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class CustomerServices : ICustomerServices
    {
        public CustomerServices(ILogger<CustomerServices> logger, HttpClient httpClient,
            IOptions<CustomerConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
        }

        private readonly ILogger<CustomerServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly CustomerConfiguration _options;

        public async Task<IList<OrigoCustomer>> GetCustomersAsync()
        {
            try
            {
                var customers = await HttpClient.GetFromJsonAsync<IList<CustomerDTO>>($"{_options.ApiPath}");
                if (customers == null) return null;
                var origoCustomers = new List<OrigoCustomer>();
                foreach (var customer in customers) origoCustomers.Add(new OrigoCustomer(customer));
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

        public async Task<OrigoCustomer> GetCustomerAsync(Guid customerId)
        {
            try
            {
                var customer = await HttpClient.GetFromJsonAsync<CustomerDTO>($"{_options.ApiPath}/{customerId}");
                return customer == null ? null : new OrigoCustomer(customer);
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

        public async Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomer);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save customer", (int)response.StatusCode);

                var customer = await response.Content.ReadFromJsonAsync<CustomerDTO>();
                return customer == null ? null : new OrigoCustomer(customer);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId)
        {
            try
            {
                var assetCategoryLifecycleTypes = await HttpClient.GetFromJsonAsync<IList<AssetCategoryLifecycleTypeDTO>>($"{_options.ApiPath}/{customerId}/AssetCategoryLifecycleTypes");
                if (assetCategoryLifecycleTypes == null) return null;
                var origoAssetCategoryLifecycleTypes = new List<OrigoAssetCategoryLifecycleType>();
                foreach (var assetCategoryLifecycleType in assetCategoryLifecycleTypes) origoAssetCategoryLifecycleTypes.Add(new OrigoAssetCategoryLifecycleType(assetCategoryLifecycleType) { IsChecked = true });
                return origoAssetCategoryLifecycleTypes;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAssetCategoryLifecycleTypesForCustomerAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAssetCategoryLifecycleTypesForCustomerAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAssetCategoryLifecycleTypesForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryLifecycleId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategoryLifecycleTypes/{assetCategoryLifecycleId}/add";
                var response = await HttpClient.PostAsJsonAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to add asset category lifecycle type configuration", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to add asset category lifecycle type configuration");
                    throw exception;
                }

                var assetCategoryLifecycleType = await response.Content.ReadFromJsonAsync<AssetCategoryLifecycleTypeDTO>();
                return assetCategoryLifecycleType == null ? null : new OrigoAssetCategoryLifecycleType(assetCategoryLifecycleType) { IsChecked = true };
            }
            catch (InvalidLifecycleTypeException exception)
            {
                _logger.LogError(exception, "AddAssetCategoryLifecycleTypeForCustomerAsync invalid lifecycletype");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddAssetCategoryLifecycleTypeForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoAssetCategoryLifecycleType> RemoveAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryLifecycleId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategoryLifecycleTypes/{assetCategoryLifecycleId}/remove";
                var response = await HttpClient.PostAsJsonAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    Exception exception = new BadHttpRequestException("Unable to remove asset category lifecycle type configuration", (int)response.StatusCode);
                    throw exception;
                }

                var assetCategoryLifecycleType = await response.Content.ReadFromJsonAsync<AssetCategoryLifecycleTypeDTO>();
                return assetCategoryLifecycleType == null ? null : new OrigoAssetCategoryLifecycleType(assetCategoryLifecycleType);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RemoveAssetCategoryLifecycleTypeForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<IList<OrigoAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId)
        {
            try
            {
                var assetCategories = await HttpClient.GetFromJsonAsync<IList<AssetCategoryTypeDTO>>($"{_options.ApiPath}/{customerId}/assetCategory");
                if (assetCategories == null) return null;
                var origoAssetCategories = new List<OrigoAssetCategoryType>();
                origoAssetCategories.AddRange(assetCategories.Select(category => new OrigoAssetCategoryType
                {
                    AssetCategoryId = category.AssetCategoryId,
                    Name = category.Name,
                    IsChecked = true,
                    LifecycleTypes = category.LifecycleTypes.Select(l => new OrigoAssetCategoryLifecycleType(l) { IsChecked = true }).ToList()
                })); ;
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

        public async Task<OrigoAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory/{assetCategoryId}/add";
                var response = await HttpClient.PostAsJsonAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to add the asset category to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to add the asset category to the customer.");
                    throw exception;
                }
                var assetCategory = await response.Content.ReadFromJsonAsync<AssetCategoryTypeDTO>();
                return assetCategory == null ? null : new OrigoAssetCategoryType
                {
                    AssetCategoryId = assetCategory.AssetCategoryId,
                    Name = assetCategory.Name,
                    IsChecked = true,
                    LifecycleTypes = assetCategory.LifecycleTypes.Select(l => new OrigoAssetCategoryLifecycleType(l)).ToList()
                };
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

        public async Task<OrigoAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, Guid assetCategoryId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/assetCategory/{assetCategoryId}/remove";
                var response = await HttpClient.PostAsJsonAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove the asset category to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove the asset category to the customer.");
                    throw exception;
                }
                var assetCategory = await response.Content.ReadFromJsonAsync<AssetCategoryTypeDTO>();
                return assetCategory == null ? null : new OrigoAssetCategoryType
                {
                    AssetCategoryId = assetCategory.AssetCategoryId,
                    Name = assetCategory.Name,
                    LifecycleTypes = assetCategory.LifecycleTypes.Select(l => new OrigoAssetCategoryLifecycleType(l)).ToList()
                };
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

        public async Task<IList<OrigoProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId)
        {
            try
            {
                var customerModules = await HttpClient.GetFromJsonAsync<IList<ModuleGroupDTO>>($"{_options.ApiPath}/{customerId}/modules");
                var customerModulesList = new List<OrigoProductModuleGroup>();

                if (customerModules == null) return null;
                customerModulesList.AddRange(customerModules.Select(module => new OrigoProductModuleGroup(module)));
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

        public async Task<OrigoProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/modules/{moduleGroupId}/add";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to add the module to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to add the module to the customer.");
                    throw exception;
                }
                var customerModules = await response.Content.ReadFromJsonAsync<ModuleGroupDTO>();
                return customerModules == null ? null : new OrigoProductModuleGroup(customerModules);
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

        public async Task<OrigoProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/modules/{moduleGroupId}/remove";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove the module  to the customer.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove the module to the customer.");
                    throw exception;
                }
                var customerModules = await response.Content.ReadFromJsonAsync<ModuleGroupDTO>();
                return customerModules == null ? null : new OrigoProductModuleGroup(customerModules);
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