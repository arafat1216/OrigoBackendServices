using Common.Extensions;
using HardwareServiceOrder.API.Controllers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HardwareServiceOrder.IntegrationTests.Controllers
{
    public class HardwareRepairNotificationControllerTests : IClassFixture<
        HardwareServiceOrderWebApplicationFactory<HardwareRepairNotificationController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
        private readonly Guid _customerId;

        public HardwareRepairNotificationControllerTests(
            HardwareServiceOrderWebApplicationFactory<HardwareRepairNotificationController> factory,
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.CUSTOMER_ONE_ID;

            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
        }

        [Fact]
        public async Task SendAssetRepairNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/asset-repair?languageCode=en";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsync(url, JsonContent.Create(""));
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }

        [Fact]
        public async Task SendLoanDeviceNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/loan-device?languageCode=en";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsync(url, JsonContent.Create(new List<int> { }));
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }

        [Fact]
        public async Task SendOrderDiscardedNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/order-discarded?languageCode=en";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsync(url, JsonContent.Create(""));
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }

        [Fact]
        public async Task SendOrderCancelledNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/order-cancelled?languageCode=en";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsync(url, JsonContent.Create(""));
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }
    }
}
