using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Response;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;

namespace OrigoApiGateway.Services
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {

        private readonly ILogger<SubscriptionManagementService> _logger;
        private readonly IMapper _mapper;
        private readonly SubscriptionManagementConfiguration _options;
        private HttpClient HttpClient { get; }
        public SubscriptionManagementService(ILogger<SubscriptionManagementService> logger, IOptions<SubscriptionManagementConfiguration> options, HttpClient httpClient, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _options = options.Value;
            HttpClient = httpClient;
        }

        public async Task<IList<OrigoOperator>> GetAllOperatorsAsync()
        {
            try
            {
                var operators = await HttpClient.GetFromJsonAsync<IList<OrigoOperator>>($"{_options.ApiPath}/operators");

                return operators;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperators failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoOperator> GetOperatorAsync(int id)
        {
            try
            {
                var operatorObject = await HttpClient.GetFromJsonAsync<OrigoOperator>($"{_options.ApiPath}/operators/{id}");

                return operatorObject;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperator failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<IList<OrigoOperator>> GetAllOperatorsForCustomerAsync(Guid organizationId)
        {
            try
            {
                var customersOperator = await HttpClient.GetFromJsonAsync<IList<OrigoOperator>>($"{_options.ApiPath}/{organizationId}/operators");

                return customersOperator;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperatorsForCustomer failed with HttpRequestException.");
                throw;
            }
        }

        public async Task AddOperatorForCustomerAsync(Guid organizationId, IList<int> operators)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{organizationId}/operators";
                var postOperator = await HttpClient.PostAsJsonAsync(requestUri, operators);
                postOperator.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddOperatorForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId)
        {
            try
            {

                string requestUri = $"{_options.ApiPath}/{organizationId}/operators/{operatorId}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
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
                string requestUri = $"{_options.ApiPath}/{organizationId}/subscription-products";
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

        public async Task<IList<OrigoSubscriptionProduct>> GetAllSubscriptionProductForCustomerAsync(Guid organizationId)
        {
            try
            {
                var subscriptionProduct = await HttpClient.GetFromJsonAsync<IList<OrigoSubscriptionProduct>>($"{_options.ApiPath}/{organizationId}/subscription-products");

                return subscriptionProduct;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetSubscriptionProductForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoSubscriptionProduct> DeleteSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionProductId)
        {
            try
            {

                var requestUri = $"{_options.ApiPath}/{organizationId}/subscription-products/{subscriptionProductId}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);
                var deletedSubscriptionProduct = await response.Content.ReadFromJsonAsync<OrigoSubscriptionProduct>();
                if (deletedSubscriptionProduct == null)
                {
                    return null;
                }

                return deletedSubscriptionProduct;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetSubscriptionProductForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoSubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionProductId, UpdateSubscriptionProduct subscriptionProduct)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{customerId}/subscription-products/{subscriptionProductId}";
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

        public async Task<IList<OrigoCustomerReferenceField>> GetAllCustomerReferenceFieldsAsync(Guid organizationId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{organizationId}/customer-reference-fields";
                var customerReferenceFieldDTOs = await HttpClient.GetFromJsonAsync<IList<CustomerReferenceFieldResponseDTO>>(requestUri);

                return _mapper.Map<List<OrigoCustomerReferenceField>>(customerReferenceFieldDTOs);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllCustomerReferenceFieldsAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoCustomerReferenceField> AddCustomerReferenceFieldAsync(Guid organizationId, NewCustomerReferenceField newCustomerReferenceField,
            string callerId)
        {
            try
            {
                var customerReferenceFieldRequest =
                    _mapper.Map<CustomerReferenceFieldPostRequestDTO>(newCustomerReferenceField);
                customerReferenceFieldRequest.CallerId = callerId;
                var requestUri = $"{_options.ApiPath}/{organizationId}/customer-reference-fields";
                var response = await HttpClient.PostAsJsonAsync(requestUri, customerReferenceFieldRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new BadHttpRequestException($"Unable to add customer reference field. Message = {errorMessage}",
                        (int)response.StatusCode);
                }

                var customerReferenceField = await response.Content.ReadFromJsonAsync<CustomerReferenceFieldResponseDTO>();
                return customerReferenceField == null ? null : _mapper.Map<OrigoCustomerReferenceField>(customerReferenceField);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"AddCustomerReferenceFieldAsync failed with HttpRequestException." );
                throw;
            }
        }

        public async Task<OrigoCustomerReferenceField> DeleteCustomerReferenceFieldAsync(Guid organizationId, int customerReferenceId, string callerId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{organizationId}/customer-reference-fields/{customerReferenceId}";
                var response = await HttpClient.DeleteAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to delete customer reference field", (int)response.StatusCode);

                var customerReferenceField = await response.Content.ReadFromJsonAsync<CustomerReferenceFieldResponseDTO>();
                return customerReferenceField == null ? null : _mapper.Map<OrigoCustomerReferenceField>(customerReferenceField);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"DeleteCustomerReferenceFieldAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task TransferSubscriptionOrderForCustomerAsync(Guid customerId, TransferFromPrivateToBusinessSubscriptionOrder order)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{customerId}/subscription-transfer";
                var postSubscription = await HttpClient.PostAsJsonAsync(requestUri, order);

                postSubscription.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddSubscriptionForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<IEnumerable<OrigoCustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId)
        {
            try
            {
                var customersOperatorAccounts = await HttpClient.GetFromJsonAsync<IList<OrigoCustomerOperatorAccount>>($"{_options.ApiPath}/{customerId}/operator-accounts");

                return customersOperatorAccounts;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperatorsForCustomer failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoCustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid customerId, OrigoCustomerOperatorAccount origoCustomerOperatorAccount)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{customerId}/operator-accounts";

                var response = await HttpClient.PostAsJsonAsync(requestUri, origoCustomerOperatorAccount);

                var newOperatorAccount = await response.Content.ReadFromJsonAsync<OrigoCustomerOperatorAccount>();

                return newOperatorAccount;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AddSubscriptionForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task DeleteOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber)
        {
            try
            {
                string requestUri = $"{_options.ApiPath}/{organizationId}/operator-accounts/{accountNumber}";

                var response = await HttpClient.SendAsync(new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                });

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetSubscriptionProductForCustomerAsync failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<IList<OrigoSubscriptionProduct>> GetAllOperatorsSubscriptionProductsAsync()
        {
            try
            {
                var operatorsSubscriptionProduct = await HttpClient.GetFromJsonAsync<IList<OrigoSubscriptionProduct>>($"{_options.ApiPath}/operators/subscription-products");

                return operatorsSubscriptionProduct;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetAllOperatorsSubscriptionProductsAsync failed with HttpRequestException.");
                throw;
            }
        }
    }
}
