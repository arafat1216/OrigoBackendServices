using System.Text;
using System.Net.Http.Json;
using HardwareServiceOrderServices.Conmodo.ApiModels;
using System.Configuration;

namespace HardwareServiceOrderServices.Conmodo
{
    /// <summary>
    ///     Responsible for performing all API requests toward Conmodo.
    /// </summary>
    internal class ApiRequests
    {
        /// <summary>
        ///     A configured and instantiated <see cref="System.Net.Http.HttpClient"/> used for all external requests.
        /// </summary>
        private HttpClient HttpClient { get; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "It is unnecessary to create a custom exception for this specific edge-case as the condition should never occur")]
        public ApiRequests(string apiUsername, string apiPassword)
        {
            string? baseUrl = ConfigurationManager.AppSettings["Conmodo:ApiBaseUrl"];
            string authenticationString = $"{apiUsername}:{apiPassword}";
            string _base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            if (string.IsNullOrEmpty(baseUrl))
                throw new ApplicationException("Conmodo API Base URL is not defined!");

            HttpClient = new HttpClient();
            HttpClient.BaseAddress = new Uri(baseUrl);
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _base64EncodedAuthenticationString);
        }

        /// <summary>
        ///     Perform a simple test-call to the API.
        /// </summary>
        /// <returns> The retrieved value. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code or an invalid response object. </exception>
        public async Task<string> TestAsync()
        {
            var response = await HttpClient.GetAsync("/test/hello");
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return responseBody;
            }
            else
            {
                var innerException = new HttpRequestException($"External response:\n{responseBody}");
                throw new HttpRequestException($"External request failed.", innerException, response.StatusCode);
            }
        }

        /// <summary>
        ///     Retrieves the status for the requested order.
        /// </summary>
        /// <param name="commId"> Conmodo's service ID. </param>
        /// <returns> An object containing the details for the requested service-order. </returns>
        public async Task<OrderResponse> GetOrderAsync(string commId)
        {
            string url = $"/order?commId={commId}";
            return await GetAsync<OrderResponse>(url);
        }

        /// <summary>
        ///     Retrieves a list of all orders with updates after the provided timestamp.
        /// </summary>
        /// <param name="since"> Conmodo's system will keep track. If provided, retrieve order updated since provided date. </param>
        /// <returns> An object containing all updated orders. </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0071:Simplify interpolation", Justification = "Results in less clear code")]
        public async Task<UpdatedOrdersResponse> GetUpdatedOrdersAsync(DateTimeOffset? since)
        {
            string url = $"/order/news";

            if (since is not null)
            {
                url += $"?since={since.Value.ToString("O")}";
            }

            return await GetAsync<UpdatedOrdersResponse>(url);
        }


        /// <summary>
        ///     Creates a new service-order.
        /// </summary>
        /// <param name="orderRequest"> The service-order that should be created. </param>
        /// <returns> The response object. </returns>
        public async Task<CreateOrderResponse> CreateServiceOrderAsync(CreateOrderRequest orderRequest)
        {
            string url = "/order";
            return await PostAsync<CreateOrderResponse, CreateOrderRequest>(url, orderRequest);
        }


        /// <summary>
        ///     Performs a GET request. 
        ///     The response is then de-serialized back to the <typeparamref name="TResult"/> object and returned.
        /// </summary>
        /// <remarks>
        ///     This should only be used when we actually expect a response-body back from the endpoint.
        /// </remarks>
        /// <typeparam name="TResult"> The response-object that is received back from the API. </typeparam>
        /// <param name="url"> The URL the request is sent to. </param>
        /// <returns> The de-serialized <typeparamref name="TResult"/> class returned from the request. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code or an invalid response object. </exception>
        private async Task<TResult> GetAsync<TResult>(string url) where TResult : class
        {
            var response = await HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<TResult>();

                if (responseBody is null)
                    throw new HttpRequestException("Response body is null.");
                else
                    return responseBody;
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var innerException = new HttpRequestException($"External response:\n{responseBody}");

                throw new HttpRequestException($"External request failed.", innerException, response.StatusCode);
            }
        }

        /// <summary>
        ///     Serializes the <paramref name="output"/>, and performs a POST request. 
        ///     The response is then de-serialized back to the <typeparamref name="TResult"/> object and returned.
        /// </summary>
        /// <remarks>
        ///     This should only be used when we actually expect a response-body back from the endpoint.
        /// </remarks>
        /// <typeparam name="TResult"> The response-object that is received back from the API. </typeparam>
        /// <typeparam name="TOutput"> The object that should be posted to the API. </typeparam>
        /// <param name="url"> The URL the request is sent to. </param>
        /// <param name="output"> The object we are transmitting to Conmodo. </param>
        /// <returns> The de-serialized <typeparamref name="TResult"/> class returned from the request. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code or an invalid response object. </exception>
        private async Task<TResult> PostAsync<TResult, TOutput>(string url, TOutput output) where TResult : class
                                                                                            where TOutput : class
        {
            var response = await HttpClient.PostAsJsonAsync(url, output);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<TResult>();

                if (responseBody is null)
                    throw new HttpRequestException("Response body is null.");
                else
                    return responseBody;
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var innerException = new HttpRequestException($"External response:\n{responseBody}");

                throw new HttpRequestException($"External request failed.", innerException, response.StatusCode);
            }
        }
    }
}
