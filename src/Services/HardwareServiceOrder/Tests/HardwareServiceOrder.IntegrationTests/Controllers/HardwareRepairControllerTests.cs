﻿using Common.Interfaces;
using HardwareServiceOrder.API.Controllers;
using HardwareServiceOrder.API.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Common.Extensions;
using System.Collections.Generic;

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

            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-User", Guid.Empty.SystemUserId().ToString());
        }

        [Fact]
        public async Task ConfigureLoanDevice()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config/loan-device";
            _testOutputHelper.WriteLine(url);
            var loanDevice = new LoanDevice("+8801724592272", "test@test.com");
            loanDevice.CallerId = _callerId;
            var request = await _httpClient.PatchAsync(url, JsonContent.Create(loanDevice));
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.NotNull(settings!.ApiUsername);
            Assert.NotNull(settings!.LoanDevice.Email);
            Assert.NotNull(settings!.LoanDevice.PhoneNumber);
        }

        [Fact]
        public async Task ConfigureSur()
        {
            var customerServiceProvider = new CustomerServiceProvider
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
            Assert.NotNull(settings!.ApiUsername);
            Assert.NotNull(settings!.LoanDevice.Email);
            Assert.NotNull(settings!.LoanDevice.PhoneNumber);
        }

        [Fact]
        public async Task GetSettings()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/config";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.NotNull(settings!.ApiUsername);
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
            var settings = await request.Content.ReadFromJsonAsync<CustomerSettingsResponseDTO>();
            Assert.NotNull(settings);
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.Equal(settings!.CustomerId, customerId);
            Assert.Null(settings!.ApiUsername);
            Assert.Null(settings!.LoanDevice.PhoneNumber);
            Assert.Null(settings!.LoanDevice.Email);
        }

        [Fact]
        public async Task GetOrders()
        {
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponseDTO>>();
            Assert.NotNull(orders);
            Assert.NotNull(orders.Items);
            Assert.NotNull(orders.Items[0].Events);
            Assert.NotNull(orders.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders.Items[0].ErrorDescription);
            Assert.NotNull(orders.Items[0].DeliveryAddress);
        }

        [Fact]
        public async Task GetMyOrders()
        {
            Guid? userId = new Guid("3286ba71-fdde-4496-94fa-36de7aa0b41e");
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?userId={userId}&page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponseDTO>>();
            Assert.NotNull(orders);
            Assert.Equal(1, orders.Items.Count);
            Assert.NotNull(orders.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders.Items[0].ErrorDescription);
            Assert.NotNull(orders.Items[0].DeliveryAddress);
        }

        [Fact]
        public async Task GetActiveOrders()
        {
            Guid? userId = new Guid("3286ba71-fdde-4496-94fa-36de7aa0b41e");
            var url = $"/api/v1/hardware-repair/{_customerId}/orders?activeOnly={true}&page=1&limit=10";
            _testOutputHelper.WriteLine(url);
            var request = await _httpClient.GetAsync(url);
            var orders = await request.Content.ReadFromJsonAsync<PagedModel<HardwareServiceOrderResponseDTO>>();
            Assert.NotNull(orders);
            Assert.NotNull(orders.Items[0].ExternalServiceManagementLink);
            Assert.NotNull(orders.Items[0].ErrorDescription);
            Assert.NotNull(orders.Items[0].DeliveryAddress);
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
        public async Task CreateHardwarRepaireOrder()
        {
            var body = new NewHardwareServiceOrder
            {
                ErrorDescription = "sd",
                OrderedBy = new ContactDetails
                {
                    FirstName = "sd",
                    LastName = "sd",
                    Id = _customerId,
                    Email = "sds@as.com",
                    PartnerId = new Guid(),
                    PartnerName = "ved",
                    PartnerOrganizationNumber = "23456",
                    OrganizationId = new Guid(),
                    OrganizationName = "AS",
                    OrganizationNumber = "12",
                    PhoneNumber = "23"
                },
                AssetInfo = new AssetInfo
                {
                    Imei = "500119468586675",
                    //AssetLifecycleId = new Guid(),
                    Accessories = new List<string>
                    {
                        "sdsd"
                    },
                    AssetCategoryId = 1,
                    Model = "wwe",
                    Brand = "wewe",
                    PurchaseDate = new DateOnly(),
                    SerialNumber = "wewew",
                    AssetLifecycleId = new Guid(),
                    AssetName = "sd"
                },
                DeliveryAddress = new DeliveryAddress
                {
                    Recipient = "fs",
                    Address1 = "f",
                    Address2 = "f",
                    City = "f",
                    Country = "FS",
                    PostalCode = "0011",
                    RecipientType = HardwareServiceOrderServices.Models.RecipientTypeEnum.Personal
                }
            };

            //var response = await client.PostAsJsonAsync($"/api/v1/hardware-repair/{_customerId}/orders", body);

            var request = $"/api/v1/hardware-repair/{_customerId}/orders";
            var response = await _httpClient.PostAsJsonAsync(request, body);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var hardwareServiceOrder = await response.Content.ReadFromJsonAsync<HardwareServiceOrderResponseDTO>();

            Assert.Equal("Registered", hardwareServiceOrder.Status);
            Assert.Equal("SUR", hardwareServiceOrder.Type);
            Assert.Equal(_customerId, hardwareServiceOrder.Owner);
        }
    
    }
}
