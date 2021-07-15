using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

        public async Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomer);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save customer", (int) response.StatusCode);

                var customer = await response.Content.ReadFromJsonAsync<CustomerDTO>();
                return customer == null ? null : new OrigoCustomer(customer);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomersAsync unknown error.");
                throw;
            }
        }
        public async Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(NewAssetCategoryLifecycleType newAssetCategoryLifecycleType)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{newAssetCategoryLifecycleType.CustomerId}/assetCategoryLifecycleTypes", newAssetCategoryLifecycleType);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save asset category lifecycletype configuration", (int) response.StatusCode);

                var assetCategoryLifecycleType = await response.Content.ReadFromJsonAsync<AssetCategoryLifecycleTypeDTO>();
                return assetCategoryLifecycleType == null ? null : new OrigoAssetCategoryLifecycleType(assetCategoryLifecycleType);
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "AddAssetCategoryLifecycleTypeForCustomerAsync unknown error.");
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
                foreach (var assetCategoryLifecycleType in assetCategoryLifecycleTypes) origoAssetCategoryLifecycleTypes.Add(new OrigoAssetCategoryLifecycleType(assetCategoryLifecycleType));
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

        public async Task<OrigoProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var customer = await HttpClient.GetFromJsonAsync<ModuleGroupDTO>($"{_options.ApiPath}/{customerId}/modules/{moduleGroupId}/add");
                return customer == null ? null : new OrigoProductModuleGroup(customer);
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

        public async Task<OrigoProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var customer = await HttpClient.GetFromJsonAsync<ModuleGroupDTO>($"{_options.ApiPath}/{customerId}/modules/{moduleGroupId}/remove");
                return customer == null ? null : new OrigoProductModuleGroup(customer);
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
    }
}