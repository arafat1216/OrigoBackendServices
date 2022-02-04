using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
      
        private readonly ILogger<SubscriptionManagementService> _logger;
        private readonly SubscriptionManagementConfiguration _options;
        private readonly IMapper _mapper;
        private HttpClient HttpClient { get; }
        public SubscriptionManagementService(ILogger<SubscriptionManagementService> logger, IOptions<SubscriptionManagementConfiguration> options, HttpClient httpClient, IMapper mapper)
        {
            _logger = logger;
            _options = options.Value;
            HttpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<IList<string>> GetAllOperators()
        {
            try
            {
                var operators = await HttpClient.GetFromJsonAsync<IList<string>>($"{_options.ApiPath}/operator");

                return operators;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperators failed with HttpRequestException.");
                throw;
            }
            
           
        }

        public async Task<OrigoOperator> GetOperator(string operatorName)
        {
            try
            {
                var operatorObject = await HttpClient.GetFromJsonAsync<OrigoOperator>($"{_options.ApiPath}/operator/{operatorName}");

                return operatorObject;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperator failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<IList<string>> GetAllOperatorsForCustomer(Guid organizationId)
        {
            try
            {
                var customersOperator = await HttpClient.GetFromJsonAsync<IList<string>>($"{_options.ApiPath}/{organizationId}/operator");

                return customersOperator;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperatorsForCustomer failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<bool> AddOperatorForCustomerAsync(Guid organizationId, IList<string> operators)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{organizationId}/operator";
                var postOperator = await HttpClient.PostAsJsonAsync(requestUri, operators);

                return postOperator.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddOperatorForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName)
        {
            try
            {

                string requestUri = $"{_options.ApiPath}/{organizationId}/operator/{operatorName}";
                var deleteOperator = await HttpClient.PostAsJsonAsync(requestUri, new StringContent(string.Empty));

                return deleteOperator.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "DeleteOperatorForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId, OrderTransferSubscription subscription)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{organizationId}/subscription";
                var postSubscription = await HttpClient.PostAsJsonAsync(requestUri, subscription);

                return postSubscription.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddSubscriptionForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid organizationId, NewSubscriptionProduct subscriptionProduct)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{organizationId}/subscriptionProducts";
                var response = await HttpClient.PostAsJsonAsync(requestUri, subscriptionProduct);

                var newSubscriptionProduct = await response.Content.ReadFromJsonAsync<OrigoSubscriptionProduct>();

                return newSubscriptionProduct;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddSubscriptionProductForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<IList<OrigoSubscriptionProduct>> GetSubscriptionProductForCustomerAsync(Guid organizationId, string operatorName)
        {
            try
            {
                var subscriptionProduct = await HttpClient.GetFromJsonAsync<IList<OrigoSubscriptionProduct>>($"{_options.ApiPath}/{organizationId}/subscriptionProducts/{operatorName}");

                return subscriptionProduct;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetSubscriptionProductForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }
    }
}
