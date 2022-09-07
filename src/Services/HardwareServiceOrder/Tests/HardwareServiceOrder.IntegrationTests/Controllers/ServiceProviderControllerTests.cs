using HardwareServiceOrder.IntegrationTests;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HardwareServiceOrder.API.Controllers.Tests
{
    public class ServiceProviderControllerTests : IClassFixture<HardwareServiceOrderWebApplicationFactory<ServiceProviderController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;
        private readonly string _controllerBaseUrl;
        private readonly Guid _callerId;

        public ServiceProviderControllerTests(HardwareServiceOrderWebApplicationFactory<ServiceProviderController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _customerId = factory.CUSTOMER_ONE_ID;

            _httpClient = factory.CreateDefaultClient();
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", _callerId.ToString());

            _controllerBaseUrl = $"api/v1/service-provider";
            _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
        }


        /// <summary> Test for <see cref="ServiceProviderController.GetAllServiceProvidersAsync(bool, bool)"/> </summary>
        [Fact(DisplayName = "Get all service-providers")]
        public async Task GetAllServiceProvidersAsync_OK_Test()
        {
            // Arrange
            Dictionary<string, string> queryParameters = new()
            {
                { "includeSupportedServiceTypes", true.ToString() },
                { "includeOfferedServiceOrderAddons", true.ToString() }
            };

            string url = await Helper.BuildRequestUrl(_controllerBaseUrl, queryParameters);
            _testOutputHelper.WriteLine($"URL: {url}");

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create("");

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ServiceProviderDTO>>();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }


        /// <summary> Test for <see cref="ServiceProviderController.GetServiceProvidersByIdAsync(int, bool, bool)"/> </summary>
        [Fact(DisplayName = "Get service-provider by ID'")]
        public async Task GetServiceProvidersByIdAsync_OK_Test()
        {
            // Arrange
            Dictionary<string, string> queryParameters = new()
            {
                { "includeSupportedServiceTypes", true.ToString() },
                { "includeOfferedServiceOrderAddons", true.ToString() }
            };

            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/{(int)ServiceProviderEnum.ConmodoNo}", queryParameters);
            _testOutputHelper.WriteLine($"URL: {url}");

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create("");

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceProviderDTO>();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }
    }
}