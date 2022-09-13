using AutoMapper;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using System.Security.Claims;
using Common.Enums;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using ServiceProvider = OrigoApiGateway.Models.HardwareServiceOrder.Backend.ServiceProvider;

#nullable enable

namespace OrigoApiGateway.Services
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly ILogger<HardwareServiceOrderService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly HardwareServiceOrderConfiguration _options;
        private readonly IAssetServices _assetServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;
        private readonly IPartnerServices _partnerServices;

        private HttpClient HttpClient => _httpClientFactory.CreateClient("hardwareserviceorderservices");


        /// <summary>
        ///     Initializes a new instance of the <see cref="HardwareServiceOrderService"/>-class using dependency-injection.
        /// </summary>
        /// <param name="logger"> A dependency-injected implementation of the <see cref="ILogger{TCategoryName}"/> interface. </param>
        /// <param name="httpClientFactory"> A dependency-injected implementation of the <see cref="IHttpClientFactory"/> interface. </param>
        /// <param name="httpContextAccessor"> A dependency-injected implementation of the <see cref="IHttpContextAccessor"/> interface. </param>
        /// <param name="mapper"> A dependency-injected implementation of the <see cref="IMapper"/> interface. </param>
        /// <param name="options"> A dependency-injected implementation of the <see cref="IOptions{TOptions}"/> interface. </param>
        /// <param name="assetServices"> A dependency-injected implementation of the <see cref="IAssetServices"/> interface. </param>
        /// <param name="userServices"> A dependency-injected implementation of the <see cref="IUserServices"/> interface. </param>
        /// <param name="customerServices"> A dependency-injected implementation of the <see cref="ICustomerServices"/> interface. </param>
        /// <param name="partnerServices"> A dependency-injected implementation of the <see cref="IPartnerServices"/> interface. </param>
        public HardwareServiceOrderService(
            ILogger<HardwareServiceOrderService> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IOptions<HardwareServiceOrderConfiguration> options,
            IAssetServices assetServices,
            IUserServices userServices,
            ICustomerServices customerServices,
            IPartnerServices partnerServices)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _options = options.Value;
            _assetServices = assetServices;
            _userServices = userServices;
            _customerServices = customerServices;
            _partnerServices = partnerServices;
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
        ///     Creates URL encoded query string using the provided <paramref name="queryParameters"/>.
        ///     This string can be attached/suffixed to all HTTP requests that requires query parameters.
        /// </summary>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the generated and URL encoded query string. </returns>
        private async Task<string> GetQueryString(Dictionary<string, string> queryParameters)
        {
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();
            return queryString;
        }


        /// <summary>
        ///     Creates a complete HTTP-request URL string by using the provided <paramref name="requestUri"/> and <paramref name="queryParameters"/>.
        /// </summary>
        /// <param name="requestUri"> The request URL. </param>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the completed request URL. </returns>
        private async Task<string> BuildRequestUrl(string requestUri, Dictionary<string, string>? queryParameters)
        {
            if (queryParameters is not null)
            {
                var queryString = await GetQueryString(queryParameters);
                requestUri += $"?{queryString}";
            }

            return requestUri;
        }


        /// <summary>
        ///     Send a HTTP-request to specified URL as an asynchronous operation.
        ///     
        ///     <para>
        ///     The request will be automatically configured to include required headers, and will process the query parameters before
        ///     appending them to the URL. </para>
        /// </summary>
        /// <param name="httpMethod"> The HTTP-method to use for the request. </param>
        /// <param name="requestUri"> The request URL. </param>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task SendRequestAsync(HttpMethod httpMethod, string requestUri, Dictionary<string, string>? queryParameters)
        {
            HttpResponseMessage response;

            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            using var request = new HttpRequestMessage(httpMethod, constructedRequestUrl);
            request.Headers.Add("X-Authenticated-UserId", GetUserId());

            response = await HttpClient.SendAsync(request);

#if DEBUG
#pragma warning disable S1481 // Unused local variables should be removed
            string? responseBodyAsString = await response.Content.ReadAsStringAsync();
#pragma warning restore S1481 // Unused local variables should be removed
#endif

            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        ///     Serializes the provided data, and send a HTTP-request to specified URL as an asynchronous operation.
        ///     
        ///     <para>
        ///     The request will be automatically configured to include required headers, and will process the query parameters before
        ///     appending them to the URL. </para>
        /// </summary>
        /// <typeparam name="TInput"> The target type the request should be serialized from. </typeparam>
        /// <param name="httpMethod"> The HTTP-method to use for the request. </param>
        /// <param name="requestUri"> The request URL. </param>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <param name="inputValue"> The item that should be serialized and to the HTTP-request. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task SendRequestAsync<TInput>(HttpMethod httpMethod, string requestUri, Dictionary<string, string>? queryParameters, TInput inputValue) where TInput : notnull
        {
            HttpResponseMessage response;

            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            using var request = new HttpRequestMessage(httpMethod, constructedRequestUrl);
            request.Headers.Add("X-Authenticated-UserId", GetUserId());
            request.Content = JsonContent.Create(inputValue);

            response = await HttpClient.SendAsync(request);

#if DEBUG
#pragma warning disable S1481 // Unused local variables should be removed
            string? responseBodyAsString = await response.Content.ReadAsStringAsync();
#pragma warning restore S1481 // Unused local variables should be removed
#endif

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        ///     Serializes the provided data, and send a HTTP-request to specified URL as an asynchronous operation.
        ///     
        ///     <para>
        ///     The request will be automatically configured to include required headers, and will process the query parameters before
        ///     appending them to the URL. </para>
        /// </summary>
        /// <typeparam name="TInput"> The target type the request should be serialized from. </typeparam>
        /// <typeparam name="TOutput"> The target type the response should be serialized to. </typeparam>
        /// <param name="httpMethod"> The HTTP-method to use for the request. </param>
        /// <param name="requestUri"> The request URL. </param>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <param name="inputValue"> The item that should be serialized and to the HTTP-request. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task results contains the de-serialized object, or <see langword="null"/>
        ///     if the body was empty, or the de-serialization failed. 
        /// </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task<TOutput?> SendRequestAsync<TInput, TOutput>(HttpMethod httpMethod, string requestUri, Dictionary<string, string>? queryParameters, TInput inputValue) where TInput : notnull
        {
            HttpResponseMessage response;

            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            using var request = new HttpRequestMessage(httpMethod, constructedRequestUrl);
            request.Headers.Add("X-Authenticated-UserId", GetUserId());
            request.Content = JsonContent.Create(inputValue);

            response = await HttpClient.SendAsync(request);

#if DEBUG
#pragma warning disable S1481 // Unused local variables should be removed
            string? responseBodyAsString = await response.Content.ReadAsStringAsync();
#pragma warning restore S1481 // Unused local variables should be removed
#endif

            response.EnsureSuccessStatusCode();

            TOutput? deserialized = await response.Content.ReadFromJsonAsync<TOutput>();
            return deserialized;
        }


        /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
        /// <typeparam name="TOutput"> The target type to deserialize to. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the de-serialized object. 
        ///     The value will be <see langword="null"/> for empty responses, or if we were unable to map the de-serialized item to the entity. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task<TOutput?> GetAsync<TOutput>(string requestUri, Dictionary<string, string>? queryParameters)
        {
            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            var response = await HttpClient.GetAsync(constructedRequestUrl);

#if DEBUG
#pragma warning disable S1481 // Unused local variables should be removed
            string? responseBodyAsString = await response.Content.ReadAsStringAsync();
#pragma warning restore S1481 // Unused local variables should be removed
#endif

            response.EnsureSuccessStatusCode();

            TOutput? deserialized = await response.Content.ReadFromJsonAsync<TOutput>();
            return deserialized;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeSupportedServiceTypes", includeSupportedServiceTypes.ToString() },
                { "includeOfferedServiceOrderAddons", includeOfferedServiceOrderAddons.ToString()}
            };

            var result = await GetAsync<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>>($"{_options.ServiceProviderApiPath}", queryParameters);

            if (result is null)
                throw new System.Text.Json.JsonException("The requested item was null, but we expected a value.");
            else
                return result;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerPortalServiceProvider>> CustomerPortalGetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons)
        {
            var unmapped = await GetAllServiceProvidersAsync(includeSupportedServiceTypes, includeOfferedServiceOrderAddons);
            var mapped = _mapper.Map<IEnumerable<CustomerPortalServiceProvider>>(unmapped);

            return mapped;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentialIndicators, bool includeActiveServiceOrderAddons)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeApiCredentialIndicators", includeApiCredentialIndicators.ToString() },
                { "includeActiveServiceOrderAddons", includeActiveServiceOrderAddons.ToString() }
            };

            var result = await GetAsync<IEnumerable<CustomerServiceProvider>>($"{_options.ConfigurationApiPath}/{organizationId}/service-provider", queryParameters);

            if (result is null)
                throw new System.Text.Json.JsonException("The requested item was null, but we expected a value.");
            else
                return result;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerPortalCustomerServiceProvider>> CustomerPortalGetCustomerServiceProvidersAsync(Guid organizationId, bool includeActiveServiceOrderAddons)
        {
            var unmapped = await GetCustomerServiceProvidersAsync(organizationId, false, includeActiveServiceOrderAddons);
            var mapped = _mapper.Map<IEnumerable<CustomerPortalCustomerServiceProvider>>(unmapped);

            return mapped;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<UserPortalCustomerServiceProvider>> UserPortalGetCustomerServiceProvidersAsync(Guid organizationId, bool includeActiveServiceOrderAddons)
        {
            var unmapped = await GetCustomerServiceProvidersAsync(organizationId, false, includeActiveServiceOrderAddons);
            var mapped = _mapper.Map<IEnumerable<UserPortalCustomerServiceProvider>>(unmapped);

            return mapped;
        }



        /// <inheritdoc/>
        public async Task DeleteApiCredentialsAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "serviceTypeId", serviceTypeId?.ToString() ?? string.Empty }
            };

            await SendRequestAsync(HttpMethod.Delete, $"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/credentials", queryParameters);
        }


        /// <inheritdoc/>
        public async Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, NewApiCredential apiCredential)
        {
            await SendRequestAsync(HttpMethod.Put, $"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/credentials", null, apiCredential);
        }


        /// <inheritdoc/>
        public async Task AddServiceAddonFromBackofficeAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds)
        {
            await SendRequestAsync(HttpMethod.Patch, $"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/addons", null, newServiceOrderAddonIds);
        }

        /// <inheritdoc/>
        public async Task AddServiceAddonFromCustomerPortalAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeOfferedServiceOrderAddons", true.ToString() }
            };

            ServiceProvider? serviceProvider = await SendRequestAsync<ISet<int>, ServiceProvider>(HttpMethod.Get, $"{_options.ServiceProviderApiPath}/{serviceProviderId}", queryParameters, newServiceOrderAddonIds);

            if (serviceProvider is null)
                throw new ArgumentException("The service-provider was not found.", nameof(serviceProviderId));

            var validAddonIds = serviceProvider.OfferedServiceOrderAddons!
                                               .Where(e => e.IsCustomerTogglable)
                                               .Select(e => e.Id);

            // We need to ensure the new IDs belongs to the given service-provider, and that the customer is actually allowed to add the ID!
            foreach (var newAddonId in newServiceOrderAddonIds)
            {
                bool isValid = validAddonIds.Any(i => i == newAddonId);

                if (!isValid)
                    throw new ArgumentException("The service-addon ID list contains invalid values.", nameof(newServiceOrderAddonIds));
            }

            // Place the "order" (we can use the backoffice version from here, since we have made the required checks)
            await AddServiceAddonFromBackofficeAsync(organizationId, serviceProviderId, newServiceOrderAddonIds);
        }


        /// <inheritdoc/>
        public async Task RemoveServiceAddonFromBackofficeAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds)
        {
            await SendRequestAsync(HttpMethod.Delete, $"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/addons", null, removedServiceOrderAddonIds);
        }


        /// <inheritdoc/>
        public async Task RemoveServiceAddonFromCustomerPortalAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeOfferedServiceOrderAddons", true.ToString() }
            };

            ServiceProvider? serviceProvider = await SendRequestAsync<ISet<int>, ServiceProvider>(HttpMethod.Get, $"{_options.ServiceProviderApiPath}/{serviceProviderId}", queryParameters, removedServiceOrderAddonIds);

            if (serviceProvider is null)
                throw new ArgumentException("The service-provider was not found.", nameof(serviceProviderId));

            var validAddonIds = serviceProvider.OfferedServiceOrderAddons!
                                               .Where(e => e.IsCustomerTogglable)
                                               .Select(e => e.Id);

            // We need to ensure the new IDs belongs to the given service-provider, and that the customer is actually allowed to add the ID!
            foreach (var removedAddonId in removedServiceOrderAddonIds)
            {
                bool isValid = validAddonIds.Any(i => i == removedAddonId);

                if (!isValid)
                    throw new ArgumentException("The service-addon ID list contains invalid values.", nameof(removedServiceOrderAddonIds));
            }

            // Place the "order" (we can use the backoffice version from here, since we have made the required checks)
            await RemoveServiceAddonFromBackofficeAsync(organizationId, serviceProviderId, removedServiceOrderAddonIds);
        }
        
        /// <inheritdoc/>
        public async Task<HardwareServiceOrder?> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, int serviceTypeId, NewHardwareServiceOrder model)
        {
            try
            {
                var dto = new NewHardwareServiceOrderDTO(model);

                // Verify whether the asset can be sent to repair
                var asset = (HardwareSuperType)await _assetServices.GetAssetForCustomerAsync(customerId, model.AssetId, null);

                if (asset == null)
                    throw new ArgumentException($"Asset does not exist with ID {model.AssetId}", nameof(model.AssetId));

                if (asset.AssetStatus != AssetLifecycleStatus.InUse || asset.AssetStatus != AssetLifecycleStatus.Active || asset.AssetStatus != AssetLifecycleStatus.Available)
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

                dto.ServiceTypeId = serviceTypeId; // Value "3 represents ServiceType SUR" and "2 represents ServiceType Remarketing"
                dto.ServiceProviderId = 1; //Todo: Value "1" represents the Service Provider Conmodo. In Future, this should come from Request
                dto.UserSelectedServiceOrderAddonIds = model.UserSelectedServiceOrderAddonIds;

                var hardwareServiceOrder = await SendRequestAsync<NewHardwareServiceOrderDTO, HardwareServiceOrder>(HttpMethod.Post, $"{_options.ApiPath}/{customerId}/orders", null, dto);
                return hardwareServiceOrder;
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
            catch (ArgumentException exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreateHardwareServiceOrderAsync unknown error.");
                throw;
            }
        }


        /// <inheritdoc/>
        public async Task<PagedModel<HardwareServiceOrder>> GetAllServiceOrdersForOrganizationAsync(Guid organizationId, Guid? userId, int? serviceTypeId, bool activeOnly, int page = 1, int limit = 25)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "userId", userId.ToString() ?? string.Empty },
                { "serviceTypeId", serviceTypeId.ToString() ?? string.Empty },
                { "activeOnly", activeOnly.ToString() },
                { "page", page.ToString() },
                { "limit", limit.ToString() },
            };

            var result = await GetAsync<PagedModel<HardwareServiceOrder>>($"{_options.ServiceOrderApiPath}/organization/{organizationId}/orders", queryParameters);

            // The results should never be nullable in this case, but let's check to be sure!
            if (result is null)
                throw new Exception("Failed to retrieve the paged results");

            return result;
        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder?> GetServiceOrderByIdAndOrganizationAsync(Guid organizationId, Guid serviceOrderId)
        {
            var result = await GetAsync<HardwareServiceOrder?>($"{_options.ServiceOrderApiPath}/organization/{organizationId}/orders/{serviceOrderId}", null);
            return result;
        }


    }
}
