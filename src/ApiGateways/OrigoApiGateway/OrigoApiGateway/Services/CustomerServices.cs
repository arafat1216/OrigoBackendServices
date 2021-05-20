using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;

namespace OrigoApiGateway.Services
{
    public class CustomerServices : ICustomerServices
    {
        public CustomerServices(ILogger<CustomerServices> logger, HttpClient httpClient,
            IOptions<CustomerConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            Options = options.Value;
        }

        private readonly ILogger<CustomerServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly CustomerConfiguration Options;

        public async Task<IList<OrigoCustomer>> GetCustomersAsync()
        {
            try
            {
                var customers = await HttpClient.GetFromJsonAsync<IList<CustomerDTO>>($"{Options.ApiPath}");
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
                var customer = await HttpClient.GetFromJsonAsync<CustomerDTO>($"{Options.ApiPath}/{customerId}");
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
                var response = await HttpClient.PostAsJsonAsync("{Options.ApiPath}", newCustomer);
                if (!response.IsSuccessStatusCode)
                {
                    throw new BadHttpRequestException("Unable to save customer", (int) response.StatusCode);
                }

                var customer = await response.Content.ReadFromJsonAsync<CustomerDTO>();
                return customer == null ? null : new OrigoCustomer(customer);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetCustomersAsync unknown error.");
                throw;
            }

            return null;
        }
    }
}