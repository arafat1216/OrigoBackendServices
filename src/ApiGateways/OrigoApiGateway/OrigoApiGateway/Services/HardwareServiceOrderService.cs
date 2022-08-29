using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using System.Security.Claims;

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
            response.EnsureSuccessStatusCode();

            TOutput? deserialized = await response.Content.ReadFromJsonAsync<TOutput>();
            return deserialized;
        }


        /// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent?)"/>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task PutAsync(string requestUri, Dictionary<string, string>? queryParameters)
        {
            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            using var request = new HttpRequestMessage(HttpMethod.Put, constructedRequestUrl);
            request.Headers.Add("X-Authenticated-UserId", GetUserId());

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent?)"/>
        /// <typeparam name="TInput"> The target type to deserialize to. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <param name="inputValue"> The value that should be serialized. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task PutAsync<TInput>(string requestUri, Dictionary<string, string>? queryParameters, TInput inputValue) where TInput : notnull
        {
            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            var content = JsonContent.Create(inputValue);
            content.Headers.Add("X-Authenticated-UserId", GetUserId());

            var response = await HttpClient.PutAsync(constructedRequestUrl, content);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent?)"/>
        /// <typeparam name="TInput"> The target type to deserialize to. </typeparam>
        /// <typeparam name="TOutput"> The target type to de-serialize to. </typeparam>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <param name="inputValue"> The value that should be serialized. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the de-serialized object. 
        ///     The value will be <see langword="null"/> for empty responses, or if we were unable to map the de-serialized item to the entity. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task<TOutput?> PutAsync<TInput, TOutput>(string requestUri, Dictionary<string, string>? queryParameters, TInput inputValue) where TInput : notnull
                                                                                                                                                  where TOutput : notnull
        {
            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            var content = JsonContent.Create(inputValue);
            content.Headers.Add("X-Authenticated-UserId", GetUserId());

            var response = await HttpClient.PutAsync(constructedRequestUrl, content);
            response.EnsureSuccessStatusCode();

            TOutput? deserialized = await response.Content.ReadFromJsonAsync<TOutput?>();
            return deserialized;
        }


        /// <inheritdoc cref="HttpClient.DeleteAsync(string?)"/>
        /// <param name="requestUri"> The URI the request is sent to. </param>
        /// <param name="queryParameters"> A dictionary containing all query parameters. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the de-serialized object. 
        ///     The value will be <see langword="null"/> for empty responses, or if we were unable to map the de-serialized item to the entity. </returns>
        /// <returns> A task that represents the asynchronous operation. </returns>
        private async Task DeleteAsync(string requestUri, Dictionary<string, string>? queryParameters)
        {
            string constructedRequestUrl = await BuildRequestUrl(requestUri, queryParameters);
            using var request = new HttpRequestMessage(HttpMethod.Delete, constructedRequestUrl);
            request.Headers.Add("X-Authenticated-UserId", GetUserId());

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
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
        public async Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentialIndicators)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "includeApiCredentialIndicators", includeApiCredentialIndicators.ToString() }
            };

            var result = await GetAsync<IEnumerable<CustomerServiceProvider>>($"{_options.ConfigurationApiPath}/{organizationId}/service-provider", queryParameters);

            if (result is null)
                throw new System.Text.Json.JsonException("The requested item was null, but we expected a value.");
            else
                return result;
        }


        /// <inheritdoc/>
        public async Task DeleteApiCredentialsAsync(Guid organizationId, int serviceProviderId, int serviceTypeId)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "serviceTypeId", serviceTypeId.ToString() }
            };

            await DeleteAsync($"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/credentials", queryParameters);
        }


        /// <inheritdoc/>
        public async Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, NewApiCredential apiCredential)
        {
            await PutAsync($"{_options.ConfigurationApiPath}/{organizationId}/service-provider/{serviceProviderId}/credentials", null, apiCredential);
        }
    }
}
