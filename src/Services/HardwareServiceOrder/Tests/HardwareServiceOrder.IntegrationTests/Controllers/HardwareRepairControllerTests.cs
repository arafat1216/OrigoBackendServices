using Common.Extensions;
using Common.Interfaces;
using HardwareServiceOrder.API.Controllers;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        private readonly Guid _userId;

        public HardwareRepairControllerTests(
            HardwareServiceOrderWebApplicationFactory<HardwareRepairController> factory,
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.CUSTOMER_ONE_ID;
            _userId = factory.USER_ID;
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
        }

        [Fact]
        public async Task ConfigureLoanDevice()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("+8801724592272", "test@test.com", true);
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.NotNull(settings?.ApiUsername);
            Assert.Equal("d723hjdfhdfnsl23sdf", settings?.ApiUsername);
            Assert.NotNull(settings?.LoanDevice?.Email);
            Assert.NotNull(settings?.LoanDevice?.PhoneNumber);
        }

        [Fact]
        public async Task ConfigureLoanDevice_Does_Not_Provide_Loan_Device_Bad_Request()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("+8801724592272", "test@test.com", false);
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Fact]
        public async Task ConfigureLoanDevice_Does_Not_Provide_Loan_Device_Bad_Request_Email_Required()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("+8801724592272", "", true);
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Fact]
        public async Task ConfigureSur()
        {
            var customerServiceProvider = new HardwareServiceOrder.API.ViewModels.CustomerServiceProvider
            {
                ApiPassword = "****",
                ApiUserName = "12345",
                ProviderId = 1
            };
            var url = $"/api/v1/hardware-repair/{_customerId}/config/sur?callerId={_callerId}";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(customerServiceProvider));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.NotNull(settings?.ApiUsername);
            Assert.Equal("12345", settings?.ApiUsername);
            Assert.NotNull(settings?.LoanDevice?.Email);
            Assert.NotNull(settings?.LoanDevice?.PhoneNumber);
        }

        [Fact]
        public async Task GetSettings()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();

            Assert.NotNull(settings);
            Assert.NotNull(settings?.ApiUsername);
            Assert.NotNull(settings?.LoanDevice?.Email);
            Assert.NotNull(settings?.LoanDevice?.PhoneNumber);
        }

        [Fact]
        public async Task ConfigureLoanDevice_Does_Not_Provide_Loan_Device()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("", "", false);
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.NotNull(settings);
            Assert.NotNull(settings!.ApiUsername);
            Assert.Empty(settings!.LoanDevice.Email);
            Assert.Empty(settings!.LoanDevice.PhoneNumber);
            Assert.False(settings.LoanDevice.ProvidesLoanDevice);
        }

        [Fact]
        public async Task GetSettings_Customer_Not_Exist()
        {
            var customerId = Guid.NewGuid();

            var url = $"/api/v1/hardware-repair/{customerId}/config";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.Equal(settings!.CustomerId, customerId);
            Assert.Null(settings?.ApiUsername);
            Assert.Null(settings?.LoanDevice?.PhoneNumber);
            Assert.Null(settings?.LoanDevice?.Email);
        }

        [Fact]
        public async Task GetOrders()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponse>>();
            Assert.NotNull(orders);
            Assert.NotNull(orders?.Items);
            Assert.NotNull(orders?.Items[0].ServiceEvents);
            Assert.NotNull(orders?.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders?.Items[0].UserDescription);
            Assert.NotNull(orders?.Items[0].DeliveryAddress);
        }

        [Fact]
        public async Task GetMyOrders()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?userId={_userId}&page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponse>>();
            Assert.NotNull(orders);
            Assert.Equal(2, orders?.Items.Count);
            Assert.NotNull(orders?.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders?.Items[0].UserDescription);
            Assert.NotNull(orders?.Items[0].DeliveryAddress);
        }

        [Fact]
        public async Task GetActiveOrders()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?activeOnly={true}&page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponse>>();
            Assert.NotNull(orders);
            Assert.NotNull(orders?.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders?.Items[0].UserDescription);
            Assert.NotNull(orders?.Items[0].DeliveryAddress);
        }

        [Fact]
        public async Task UpdateStatus()
        {
            var url = $"/api/v1/hardware-repair-order-status";
            _testOutputHelper.WriteLine(url);

            // Act
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(""));
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        }

        [Fact]
        public async Task CreateHardwareRepairOrder()
        {
            var body = new NewHardwareServiceOrder
            {
                ErrorDescription = "Something is not working! Fix plz!",
                OrderedBy = new ContactDetailsExtended
                {
                    FirstName = "John",
                    LastName = "Doe",
                    UserId = _userId,
                    Email = "user@domain.com",
                    PartnerId = Guid.NewGuid(),
                    PartnerName = "Partner AS",
                    PartnerOrganizationNumber = "123456789",
                    OrganizationId = Guid.NewGuid(),
                    OrganizationName = "Customer AS",
                    OrganizationNumber = "987654321",
                    PhoneNumber = "+4790000000"
                },
                AssetInfo = new()
                {
                    Imei = "500119468586675",
                    AssetCategoryId = 1,
                    Model = "Model",
                    Brand = "Brand",
                    PurchaseDate = DateOnly.Parse("2020-01-01"),
                    SerialNumber = "S/N-123456",
                    AssetLifecycleId = Guid.NewGuid(),
                    AssetName = "AssetName",
                    Accessories = new List<string>
                    {
                        "Charger"
                    }
                },
                DeliveryAddress = new()
                {
                    RecipientType = HardwareServiceOrderServices.Models.RecipientTypeEnum.Personal,
                    Recipient = "Recipient",
                    Address1 = "Address1",
                    Address2 = "Address2",
                    PostalCode = "0275",
                    City = "City",
                    Country = "NO"
                },
                ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo,
                ServiceTypeId = (int)ServiceTypeEnum.SUR,
                UserSelectedServiceOrderAddonIds = new HashSet<int>(){ (int)ServiceOrderAddonsEnum.CONMODO_PACKAGING }
            };

            var request = $"/api/v1/hardware-repair/{_customerId}/orders";
            var response = await _httpClient.PostAsJsonAsync(request, body);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var hardwareServiceOrder = await response.Content.ReadFromJsonAsync<HardwareServiceOrderResponse>();

            Assert.Equal((int)ServiceStatusEnum.Registered, hardwareServiceOrder?.StatusId);
            Assert.Equal((int)ServiceTypeEnum.SUR, hardwareServiceOrder?.ServiceTypeId);
            Assert.Equal(_customerId, hardwareServiceOrder?.CustomerId);
        }

    }
}
