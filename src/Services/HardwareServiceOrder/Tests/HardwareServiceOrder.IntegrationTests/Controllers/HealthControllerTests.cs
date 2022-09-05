using HardwareServiceOrder.API.Controllers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HardwareServiceOrder.IntegrationTests.Controllers
{
    public class HealthControllerTests : IClassFixture<HardwareServiceOrderWebApplicationFactory<HealthController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly string url = $"/api/v1/health";


        public HealthControllerTests(HardwareServiceOrderWebApplicationFactory<HealthController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
        }


        /// <summary>
        ///     Makes sure that the 'X-Authenticated-UserId' is correctly updated between API calls.
        ///     This is done by doing several requests, using different values. <para>
        ///     
        ///     We want to ensure that the dependency-injection correctly applies and returns the expected
        ///     value for the current caller, ensuring we don't accidentally set it to an incorrect lifetime
        ///     in the injections, that would result in a fixed value being applied. </para>
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test if 'X-Authenticated-UserId' changes")]
        public async Task TestThatAuthenticatedUserIdChangesAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            HttpResponseMessage firstResponse, secondResponse;
            string firstJsonResponse, secondJsonResponse;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "10000000-0000-0000-0000-000000000000");
                firstResponse = await _httpClient.SendAsync(requestMessage);

                firstJsonResponse = await firstResponse.Content.ReadAsStringAsync();
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "20000000-0000-0000-0000-000000000000");
                secondResponse = await _httpClient.SendAsync(requestMessage);

                secondJsonResponse = await secondResponse.Content.ReadAsStringAsync();
            }

            // Asserts
            Assert.Contains("10000000-0000-0000-0000-000000000000", firstJsonResponse);
            Assert.Contains("20000000-0000-0000-0000-000000000000", secondJsonResponse);
        }


        /// <summary>
        ///     Verifies the default value for the 'X-Authenticated-UserId' header parameter, where the HTTP request 
        ///     don't supply the value. This is tested on a API endpoint method (GET) that do not require the value
        ///     to be supplied.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test default 'X-Authenticated-UserId' value")]
        public async Task TestDefaultUserIdAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            HttpResponseMessage response;
            string jsonResponse;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Clear();
                response = await _httpClient.SendAsync(requestMessage);

                jsonResponse = await response.Content.ReadAsStringAsync();
            }

            // Asserts
            Assert.Contains(Guid.Empty.ToString(), jsonResponse);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for GET-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test GET request w/o headers")]
        public async Task TestGetRequestWithoutHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Clear();
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for GET-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test GET request w/headers")]
        public async Task TestGetRequestWithHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            HttpResponseMessage responseWithHeaders;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "b94f4697-9439-47b5-884a-906ca6bf909f");
                responseWithHeaders = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.OK, responseWithHeaders.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for POST-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test POST request w/o headers")]
        public async Task TestPostRequestWithoutHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for POST-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test POST request w/headers")]
        public async Task TestPostRequestWithHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "b94f4697-9439-47b5-884a-906ca6bf909f");
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for PUT-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test PUT request w/o headers")]
        public async Task TestPutRequestWithoutHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for PUT-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test PUT request w/headers")]
        public async Task TestPutRequestWithHeaderAsync()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "b94f4697-9439-47b5-884a-906ca6bf909f");
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for PATCH-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test PATCH request w/o headers")]
        public async Task TestPatchRequestWithoutHeader()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage responseNoHeaders;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                responseNoHeaders = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, responseNoHeaders.StatusCode);
        }


        /// <summary>
        ///     Checks if the <see cref="HardwareServiceOrderServices.Infrastructure.IApiRequesterService"/> in the controller(s)
        ///     correctly applies the header requirement checks for PATCH-requests.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Test PATCH request w/headers")]
        public async Task TestPatchRequestWithHeader()
        {
            // Arrange
            _testOutputHelper.WriteLine($"URL: {url}");
            var jsonContent = JsonContent.Create("");
            HttpResponseMessage response;

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                requestMessage.Content = jsonContent;
                requestMessage.Headers.Clear();
                requestMessage.Headers.Add("X-Authenticated-UserId", "b94f4697-9439-47b5-884a-906ca6bf909f");
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}
