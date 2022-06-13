using Common.Interfaces;
using HardwareServiceOrder.API.Controllers;
using HardwareServiceOrder.API.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HardwareServiceOrder.IntegrationTests.Controllers
{
    public class HardwareRepairControllerTests : IClassFixture<
        HardwareServiceOrderWebApplicationFactory<HardwareRepairController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
        private readonly Guid _customerId;
        public HardwareRepairControllerTests(
            HardwareServiceOrderWebApplicationFactory<HardwareRepairController> factory,
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.CUSTOMER_ONE_ID;
        }

        [Fact]
        public async Task ConfigureLoanDevice()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("+8801724592272", "test@test.com");
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettings>();
            Assert.NotNull(settings);
            Assert.Null(settings!.ServiceId);
            Assert.NotNull(settings!.LoanDevice.Email);
            Assert.NotNull(settings!.LoanDevice.PhoneNumber);
        }

        [Fact]
        public async Task ConfigureSur()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/sur?callerId={_callerId}";
            _testOutputHelper.WriteLine(url);
            var dto = new CustomerSettings
            {
                ServiceId = "ServiceId",
                AssetCategoryIds = new System.Collections.Generic.List<int> { 1, 2 },
                ProviderId = 1,
                CustomerId = _customerId,
            };
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(dto));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettings>();
            Assert.NotNull(settings);
            Assert.NotNull(settings!.ServiceId);
            Assert.NotNull(settings!.LoanDevice.Email);
            Assert.NotNull(settings!.LoanDevice.PhoneNumber);
        }

        [Fact]
        public async Task GetSettings()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettings>();
            Assert.NotNull(settings);
            Assert.NotNull(settings!.ServiceId);
            Assert.NotNull(settings!.LoanDevice.Email);
            Assert.NotNull(settings!.LoanDevice.PhoneNumber);
        }

        [Fact]
        public async Task GetSettings_Customer_Not_Exist()
        {
            var customerId = Guid.NewGuid();

            var url = $"/api/v1/hardware-repair/{customerId}/config";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettings>();
            Assert.NotNull(settings);
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.Equal(settings!.CustomerId, customerId);
            Assert.Null(settings!.ServiceId);
            Assert.Null(settings!.LoanDevice.PhoneNumber);
            Assert.Null(settings!.LoanDevice.Email);
        }

        [Fact]
        public async Task GetOrder()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponseDTO>>();
            Assert.NotNull(orders);
            Assert.Equal(1, orders.Items.Count);
        }
    }
}
