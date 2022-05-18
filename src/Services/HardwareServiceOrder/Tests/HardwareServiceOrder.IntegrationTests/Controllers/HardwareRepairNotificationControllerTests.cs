using HardwareServiceOrder.API.Controllers;
using HardwareServiceOrderServices.Email.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
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
        }

        [Fact]
        public async Task SendAssetRepairNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/asset-repair?languageCode=EN";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsJsonAsync(url, new List<int> { 1,2});
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }

        [Fact]
        public async Task SendLoanDeviceNotification()
        {
            var url = $"/api/v1/hardware-repair-notifications/loan-device?languageCode=EN";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PostAsJsonAsync(url, new List<int> { 1, 2 });
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }
    }
}
