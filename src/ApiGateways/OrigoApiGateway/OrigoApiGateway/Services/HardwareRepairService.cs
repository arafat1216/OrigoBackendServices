using Common.Enums;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using System.Security.Claims;

#nullable enable

namespace OrigoApiGateway.Services
{
    [Obsolete("This is superseded by the new 'Hardware Service' service-class. All new functionality should instead be placed in that one.")]
    public class HardwareRepairService : IHardwareRepairService
    {
        private readonly ILogger<HardwareRepairService> _logger;
        private readonly HardwareServiceOrderConfiguration _options;
        private readonly IAssetServices _assetServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;
        private readonly IPartnerServices _partnerServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        private HttpClient HttpClient => _httpClientFactory.CreateClient("hardwareserviceorderservices");

        public HardwareRepairService(
            ILogger<HardwareRepairService> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<HardwareServiceOrderConfiguration> options,
            IAssetServices assetServices,
            IUserServices userServices,
            ICustomerServices customerServices,
            IPartnerServices partnerServices,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _options = options.Value;
            _assetServices = assetServices;
            _userServices = userServices;
            _customerServices = customerServices;
            _partnerServices = partnerServices;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///     Accesses the current <see cref="HttpContent"/>, and retrieves the user-ID for the authenticated user.
        /// </summary>
        /// <returns> The user-ID for the authenticated user. </returns>
        private string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
        }

        /// <summary>
        ///     Serializes <paramref name="inputValue"/> and adds the dynamic HTTP header values before sending a PATCH 
        ///     request to the underlaying microservice. 
        /// </summary>
        /// <typeparam name="T"> The <see cref="Type"/> that will be serialized. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="inputValue"> The value that should be serialized. </param>
        /// <returns> The <see cref="HttpResponseMessage"/> that was received from the microservice. </returns>
        private async Task<HttpResponseMessage> PatchAsync<T>(string? requestUri, T? inputValue)
        {
            var content = JsonContent.Create(inputValue);
            content.Headers.Add("X-Authenticated-UserId", GetUserId());

            return await HttpClient.PatchAsync(requestUri, content);
        }


        /// <summary>
        ///     Serializes <paramref name="inputValue"/> and adds the dynamic HTTP header values before sending a POST 
        ///     request to the underlaying microservice. 
        /// </summary>
        /// <typeparam name="T"> The <see cref="Type"/> that will be serialized. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="inputValue"> The value that should be serialized. </param>
        /// <returns> The <see cref="HttpResponseMessage"/> that was received from the microservice. </returns>
        private async Task<HttpResponseMessage> PostAsync<T>(string? requestUri, T? inputValue)
        {
            var content = JsonContent.Create(inputValue);
            content.Headers.Add("X-Authenticated-UserId", GetUserId());

            return await HttpClient.PostAsync(requestUri, content);
        }


        /*
         * Old HW Repair methods
         */

        public async Task<CustomerSettings?> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice)
        {
            try
            {
                var request = await PatchAsync($"{_options.ApiPath}/{customerId}/config/loan-device", loanDevice);
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

        public async Task<CustomerSettings?> ConfigureServiceIdAsync(Guid customerId, string serviceId)
        {
            try
            {
                var dto = new CustomerServiceProviderDTO
                {
                    ProviderId = 1, // ConmodoNo
                    ApiUserName = serviceId
                };

                var request = await PatchAsync($"{_options.ApiPath}/{customerId}/config/sur", dto);
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

        public async Task<CustomerSettings?> GetSettingsAsync(Guid customerId)
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

        public async Task<HardwareServiceOrder?> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model)
        {
            try
            {
                var dto = new NewHardwareServiceOrderDTO(model);

                // Verify whether the asset can be sent to repair
                var asset = (HardwareSuperType)await _assetServices.GetAssetForCustomerAsync(customerId, model.AssetId, null);

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

                // Get owner information
                userId = asset.AssetHolderId ?? userId;
                var userInfo = await _userServices.GetUserAsync(userId);

                // Get organization information
                var organization = await _customerServices.GetCustomerAsync(customerId);

                if (organization == null)
                    throw new ArgumentException($"Unable retrieve organization for customerID {customerId}", nameof(customerId));

                if (organization.PartnerId == null)
                    throw new ArgumentException($"There is no partner associated with customerID {customerId}", nameof(customerId));

                // Get partner information
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


                var request = await PostAsync($"{_options.ApiPath}/{customerId}/orders", dto);
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

        public async Task<HardwareServiceOrder?> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
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

        public async Task<PagedModel<HardwareServiceOrder>?> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, int? serviceTypeId, bool activeOnly, int page = 1, int limit = 25)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<PagedModel<HardwareServiceOrder>>($"{_options.ApiPath}/{customerId}/orders?userId={userId}&serviceTypeId={serviceTypeId}&activeOnly={activeOnly}&page={page}&limit={limit}");
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
    }
}
