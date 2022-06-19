using Common.Enums;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

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
                var dto = new CustomerServiceProviderDTO
                {
                    ProviderId = 1, //ConmodoNo
                    ApiUserName = serviceId
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

        public async Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model)
        {
            try
            {
                var dto = new NewHardwareServiceOrderDTO(model);

                //Verify whether the asset can be sent to repair
                var asset = (HardwareSuperType)await _assetServices.GetAssetForCustomerAsync(customerId, model.AssetId);

                if (asset == null)
                    throw new ArgumentException($"Asset does not exist with ID {model.AssetId}", nameof(model.AssetId));

                if (asset.AssetStatus != AssetLifecycleStatus.InUse)
                {
                    throw new ArgumentException("This asset cannot be sent to repair.");
                }

                dto.AssetInfo = new AssetInfo
                {
                    AssetCategoryId = asset.AssetCategoryId,
                    AssetLifecycleId = asset.Id,
                    Brand = asset.Brand,
                    Model = asset.ProductName,
                    PurchaseDate = DateOnly.FromDateTime(asset.PurchaseDate),
                    Imei = $"{asset.Imei.FirstOrDefault()}",
                    SerialNumber = asset.SerialNumber
                };

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

                dto.OrderedBy = new ContactDetailsExtended(
                    userId,
                    userInfo.FirstName,
                    userInfo.LastName,
                    userInfo.Email,
                    userInfo.MobileNumber,
                    organization.OrganizationId,
                    organization.Name,
                    organization.OrganizationNumber,
                    partner.Id,
                    partner.Name,
                    partner.OrganizationNumber
                );

                var request = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/orders", JsonContent.Create(dto));
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
                var order = await HttpClient.GetFromJsonAsync<HardwareServiceOrder>($"{_options.ApiPath}/{customerId}/orders/{orderId}");
                return order;
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

        public async Task<PagedModel<HardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, int page = 1, int limit = 25)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<PagedModel<HardwareServiceOrder>>($"{_options.ApiPath}/{customerId}/orders?userId={userId}&activeOnly={activeOnly}&page={page}&limit={limit}");
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
