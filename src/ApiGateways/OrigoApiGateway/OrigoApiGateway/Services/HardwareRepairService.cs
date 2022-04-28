using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.HardwareServiceOrder;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class HardwareRepairService : IHardwareRepairService
    {
        private readonly ILogger<HardwareRepairService> _logger;
        private readonly HardwareServiceOrderConfiguration _options;

        private HttpClient HttpClient { get; }

        public HardwareRepairService(ILogger<HardwareRepairService> logger, HttpClient httpClient,
         IOptions<HardwareServiceOrderConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
        }

        public async Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice)
        {
            try
            {
                var request = await HttpClient.PatchAsync($"{_options.ApiPath}/{customerId}/config/loan-device", JsonContent.Create(loanDevice));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<CustomerSettings>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync unknown error.");
                throw;
            }
        }

        public async Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId)
        {
            try
            {
                var request = await HttpClient.PatchAsync($"{_options.ApiPath}/{customerId}/config/sur", JsonContent.Create(serviceId));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<CustomerSettings>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "ConfigureServiceIdAsync unknown error.");
                throw;
            }
        }

        public async Task<CustomerSettings> GetSettingsAsync(Guid customerId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<CustomerSettings>($"{_options.ApiPath}/{customerId}/config");
                return response;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetSettingsAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetSettingsAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetSettingsAsync unknown error.");
                throw;
            }
        }

        public async Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrder model)
        {
            try
            {
                var request = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/orders", JsonContent.Create(model));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<HardwareServiceOrder>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "CreateHardwareServiceOrderAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "CreateHardwareServiceOrderAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateHardwareServiceOrderAsync unknown error.");
                throw;
            }
        }

        public async Task<HardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HardwareServiceOrder>($"{_options.ApiPath}/{customerId}/orders/{orderId}");
                return response;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderAsync unknown error.");
                throw;
            }
        }

        public async Task<List<HardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<List<HardwareServiceOrder>>($"{_options.ApiPath}/{customerId}/orders");
                return response;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrdersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrdersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrdersAsync unknown error.");
                throw;
            }
        }

        public async Task<HardwareServiceOrder> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrder model)
        {
            try
            {
                var request = await HttpClient.PatchAsync($"{_options.ApiPath}/{customerId}/orders", JsonContent.Create(model));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<HardwareServiceOrder>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "UpdateHardwareServiceOrderAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "UpdateHardwareServiceOrderAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateHardwareServiceOrderAsync unknown error.");
                throw;
            }
        }

        public async Task<List<HardwareServiceOrderLog>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<List<HardwareServiceOrderLog>>($"{_options.ApiPath}/{customerId}/orders/{orderId}/logs");
                return response;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderLogsAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderLogsAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetHardwareServiceOrderLogsAsync unknown error.");
                throw;
            }
        }
    }
}
