using Microsoft.Extensions.Options;

#nullable enable

namespace OrigoApiGateway.Services
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly ILogger<HardwareServiceOrderService> _logger;
        private readonly HardwareServiceOrderConfiguration _options;
        private readonly IAssetServices _assetServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;
        private readonly IPartnerServices _partnerServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        private HttpClient HttpClient => _httpClientFactory.CreateClient("hardwareserviceorderservices");

        public HardwareServiceOrderService(
            ILogger<HardwareServiceOrderService> logger,
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
        ///     Creates URL encoded query string containing all query parameters. 
        ///     This string can be attached/suffixed on all HTTP requests that requires query parameters.
        /// </summary>
        /// <param name="queryParameters"> The dictionary containing all query names and values. </param>
        /// <returns> The generated and URL encoded query string. </returns>
        private async Task<string> GetQueryString(Dictionary<string, string> queryParameters)
        {
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();
            return queryString;
        }


        /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
        /// <typeparam name="T"> The target type to deserialize to. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <returns> The task object representing the asynchronous operation. </returns>
        private async Task<T?> GetAsync<T>(string requestUri, Dictionary<string, string>? queryParameters)
        {
            if (queryParameters is not null)
            {
                var queryString = await GetQueryString(queryParameters);
                requestUri += $"?{queryString}";
            }

            var request = await HttpClient.GetAsync(requestUri);
            request.EnsureSuccessStatusCode();
            return await request.Content.ReadFromJsonAsync<T>();
        }



        public async Task<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>?> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeSupportedServiceTypes", includeSupportedServiceTypes.ToString() },
                { "includeOfferedServiceOrderAddons", includeOfferedServiceOrderAddons.ToString()}
            };

            return await GetAsync<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>>($"{_options.ServiceProviderApiPath}", queryParameters);
        }

    }
}
