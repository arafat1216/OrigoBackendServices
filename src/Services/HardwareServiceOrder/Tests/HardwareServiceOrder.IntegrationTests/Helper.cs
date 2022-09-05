using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HardwareServiceOrder.IntegrationTests
{
    /// <summary>
    ///     Various helpers used for performing API-requests.
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        ///     Creates a completed HTTP-request URL using the provided API-endpoint URL and query-parameters.
        /// </summary>
        /// <param name="requestUri"> The URL to the API-endpoint. </param>
        /// <param name="queryParameters"> The query parameters. </param>
        /// <returns> A task containing the asynchronous operation. The task results contains a string with the constructed URL. </returns>
        internal static async Task<string> BuildRequestUrl(string requestUri, Dictionary<string, string>? queryParameters)
        {
            if (queryParameters is not null)
            {
                var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
                var queryString = await dictFormUrlEncoded.ReadAsStringAsync();
                requestUri += $"?{queryString}";
            }

            return requestUri;
        }
    }
}
