using Common.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
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
        private readonly IAssetServices _assetServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;
        private readonly IPartnerServices _partnerServices;

        private HttpClient HttpClient { get; }

        public HardwareRepairService(
            ILogger<HardwareRepairService> logger,
            HttpClient httpClient,
            IOptions<HardwareServiceOrderConfiguration> options,
            IAssetServices assetServices,
            IUserServices userServices,
            ICustomerServices customerServices,
            IPartnerServices partnerServices)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _assetServices = assetServices;
            _userServices = userServices;
            _customerServices = customerServices;
            _partnerServices = partnerServices;
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
                //TODO: Hardcoded values should be replaced in later PR
                var dto = new NewCustomerSettingsDTO
                {
                    CustomerId = customerId,
                    ServiceId = serviceId,
                    ProviderId = 1, //Provider identifier for conmodo
                    AssetCategoryIds = new List<int>
                    {
                        1, // MobilePhone
                        2 // Tablet
                    }
                };

                var request = await HttpClient.PatchAsync($"{_options.ApiPath}/{customerId}/config/sur", JsonContent.Create(dto));
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

        public async Task<OrigoHardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model)
        {
            try
            {
                var dto = new NewHardwareServiceOrderDTO(model);

                //Verify whether the asset can be sent to repair
                var asset = await _assetServices.GetAssetForCustomerAsync(customerId, model.AssetInfo.AssetLifecycleId);

                if (asset.AssetStatus != AssetLifecycleStatus.InUse)
                {
                    throw new ArgumentException("This asset cannot be sent to repair.");
                }

                //Get owner information
                userId = asset.AssetHolderId ?? userId;
                var userInfo = await _userServices.GetUserAsync(userId);

                //Get organization information
                var organization = await _customerServices.GetCustomerAsync(customerId);

                if (organization == null)
                    throw new ArgumentException($"Unable retrieve organization for customerID {customerId}", nameof(customerId));

                if (organization.PartnerId == null)
                    throw new ArgumentException($"There is no partner associated with customerID {customerId}", nameof(customerId));

                //Get partner information
                var partner = await _partnerServices.GetPartnerAsync(organization.PartnerId.GetValueOrDefault());

                if (partner == null)
                    throw new ArgumentException($"No partner is for customerId {customerId}", nameof(customerId));

                dto.OrderedBy = new OrderedByUserDTO
                {
                    Email = userInfo.Email,
                    FistName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Id = userId,
                    PhoneNumber = userInfo.MobileNumber,
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    OrganizationNumber = organization.OrganizationNumber,
                    PartnerId = partner.Id,
                    PartnerName = partner.Name,
                    PartnerOrganizationNumber = partner.OrganizationNumber
                };

                var request = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/orders", JsonContent.Create(dto));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<OrigoHardwareServiceOrder>();
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

        public async Task<OrigoHardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<OrigoHardwareServiceOrder>($"{_options.ApiPath}/{customerId}/orders/{orderId}");
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

        public async Task<List<OrigoHardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<List<OrigoHardwareServiceOrder>>($"{_options.ApiPath}/{customerId}/orders");
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

        public async Task<OrigoHardwareServiceOrder> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, NewHardwareServiceOrder model)
        {
            try
            {
                var request = await HttpClient.PatchAsync($"{_options.ApiPath}/{customerId}/orders", JsonContent.Create(model));
                request.EnsureSuccessStatusCode();
                return await request.Content.ReadFromJsonAsync<OrigoHardwareServiceOrder>();
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
