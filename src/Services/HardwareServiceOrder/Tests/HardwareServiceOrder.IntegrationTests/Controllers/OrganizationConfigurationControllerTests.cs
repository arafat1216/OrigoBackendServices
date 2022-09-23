using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrder.IntegrationTests;
using HardwareServiceOrderServices.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HardwareServiceOrder.API.Controllers.Tests
{
    public class OrganizationConfigurationControllerTests : IClassFixture<HardwareServiceOrderWebApplicationFactory<OrganizationConfigurationController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;
        private readonly string _controllerBaseUrl;
        private readonly Guid _callerId;

        public OrganizationConfigurationControllerTests(HardwareServiceOrderWebApplicationFactory<OrganizationConfigurationController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _customerId = factory.CUSTOMER_ONE_ID;

            _httpClient = factory.CreateDefaultClient();
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", _callerId.ToString());

            _controllerBaseUrl = $"api/v1/configuration/organization/{_customerId}";
            _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
        }




        [Fact(DisplayName = "Get all 'CustomerServiceProvider' for a customer")]
        public async Task GetCustomerServiceProvidersAsync_OK_Test()
        {
            // Arrange
            Dictionary<string, string> queryParameters = new()
            {
                { "includeApiCredentialIndicators", true.ToString() },
                { "includeActiveServiceOrderAddons", true.ToString() }
            };

            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider", queryParameters);
            _testOutputHelper.WriteLine($"URL: {url}");

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create("");

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ViewModels.CustomerServiceProvider>>();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }


        [Fact(DisplayName = "Get 'CustomerServiceProvider' by ID")]
        public async Task GetCustomerServiceProviderByIdAsync_OK_Test()
        {
            // Arrange
            Dictionary<string, string> queryParameters = new()
            {
                { "includeApiCredentialIndicators", true.ToString() },
                { "includeActiveServiceOrderAddons", true.ToString() }
            };

            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}", queryParameters);
            _testOutputHelper.WriteLine($"URL: {url}");

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create("");

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            var result = await response.Content.ReadFromJsonAsync<ViewModels.CustomerServiceProvider>();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }


        [Fact(DisplayName = "Add or update 'ApiCredential'")]
        public async Task AddOrUpdateApiCredentialsAsync_204_Test()
        {
            // Arrange
            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/credentials", null);
            _testOutputHelper.WriteLine($"URL: {url}");

            NewApiCredential apiCredential = new()
            {
                ApiUsername = "Username",
                ApiPassword = "Password",
                ServiceTypeId = null
            };

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create(apiCredential);

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        
        [Theory(DisplayName = "Add or update 'ApiCredential' with supported ServiceType")]
        [InlineData(ServiceTypeEnum.SUR)]
        [InlineData(ServiceTypeEnum.Remarketing)]
        public async Task AddOrUpdateApiCredentialsAsync_With_Supported_ServiceType_Test(ServiceTypeEnum serviceType)
        {
            // Arrange
            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/credentials", null);
            _testOutputHelper.WriteLine($"URL: {url}");

            NewApiCredential apiCredential = new()
            {
                ApiUsername = "Username",
                ApiPassword = "Password",
                ServiceTypeId = (int)serviceType
            };

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create(apiCredential);

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        
        [Theory(DisplayName = "Add or update 'ApiCredential' with not supported ServiceType")]
        [InlineData((int)ServiceTypeEnum.Null)]
        [InlineData((int)ServiceTypeEnum.PreSwap)]
        [InlineData(15)]
        public async Task AddOrUpdateApiCredentialsAsync_With_NotSupported_ServiceType_Test(int serviceType)
        {
            // Arrange
            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/credentials", null);
            _testOutputHelper.WriteLine($"URL: {url}");

            NewApiCredential apiCredential = new()
            {
                ApiUsername = "Username",
                ApiPassword = "Password",
                ServiceTypeId = serviceType
            };

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create(apiCredential);

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact(DisplayName = "Delete 'ApiCredential'")]
        public async Task DeleteApiCredentialsAsync_204_Test()
        {
            // Arrange
            Dictionary<string, string> queryParameters = new()
            {
                { "serviceTypeId", $"{(int)ServiceTypeEnum.SUR}" }
            };

            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/credentials", queryParameters);
            _testOutputHelper.WriteLine($"URL: {url}");

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create("");

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact(DisplayName = "Add 'ServiceOrderAddons' to 'CustomerServiceProvider'")]
        public async Task AddServiceOrderAddonsToCustomerServiceProviderAsync_204_Test()
        {
            // Arrange
            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/addons", null);
            _testOutputHelper.WriteLine($"URL: {url}");

            HashSet<int> newServiceOrderAddonIds = new() { 1 };

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create(newServiceOrderAddonIds);

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }


        [Fact(DisplayName = "Remove 'ServiceOrderAddons' from 'CustomerServiceProvider'")]
        public async Task RemoveServiceOrderAddonsFromCustomerServiceProviderAsync_204_Test()
        {
            // Arrange
            string url = await Helper.BuildRequestUrl($"{_controllerBaseUrl}/service-provider/{(int)ServiceProviderEnum.ConmodoNo}/addons", null);
            _testOutputHelper.WriteLine($"URL: {url}");

            HashSet<int> removedServiceOrderAddonIds = new() { 1 };

            HttpResponseMessage response;
            var jsonContent = JsonContent.Create(removedServiceOrderAddonIds);

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                requestMessage.Content = jsonContent;
                response = await _httpClient.SendAsync(requestMessage);
            }

            // Asserts
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}