using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SubscriptionManagement.API.Controllers;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.ServiceModels;
using Xunit;
using Xunit.Abstractions;

namespace SubscriptionManagement.IntegrationTests.Controllers;

public class
    SubscriptionManagementControllerTests : IClassFixture<
        SubscriptionManagementWebApplicationFactory<SubscriptionManagementController>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;
    private readonly int _operatorId;
    private readonly int _operatorAccountId;
    private readonly int _subscriptionProductId;


    public SubscriptionManagementControllerTests(
        SubscriptionManagementWebApplicationFactory<SubscriptionManagementController> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _subscriptionProductId = factory.CUSTOMER_SUBSCRIPTION_PRODUCT_ID;
        _organizationId = factory.ORGANIZATION_ID;
        _operatorAccountId = factory.OPERATOR_ACCOUNT_ID;
        _operatorId = factory.FIRST_OPERATOR_ID;
    }

    [Fact]
    public async Task GetAllOperators_ReturnsSeededOperators()
    {
        var modelList =
            await _httpClient.GetFromJsonAsync<IList<ExpectedGetOperatorModel>>(
                "/api/v1/SubscriptionManagement/operators");

        Assert.Equal(4, modelList?.Count);
    }

    [Fact]
    public async Task CreateAndDeleteCustomerSubscriptionProduct()
    {
        var customerSubscriptionProduct = new NewSubscriptionProduct
        {
            Name = "A customer subscription product",
            OperatorId = _operatorId,
            DataPackages = new List<string> { "20GB" },
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(customerSubscriptionProduct));
        var createResponse = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products", customerSubscriptionProduct);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var modelList =
            await _httpClient.GetFromJsonAsync<IList<CustomerSubscriptionProductDTO>>(
                $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products");
        var retrievedNewCustomerSubscriptionProduct = modelList!.FirstOrDefault(p =>
            p.Name == customerSubscriptionProduct.Name && !p.isGlobal);
        var deleteResponse = await _httpClient.DeleteAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products/{retrievedNewCustomerSubscriptionProduct!.Id}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task PostTransferPrivateToBusinessSubscriptionOrder_ReturnsCreated()
    {
        var newTransferToBusinessFromPrivate = new TransferToBusinessSubscriptionOrderDTO
        {
            BusinessSubscription =
                new BusinessSubscriptionDTO
                {
                    Name = "Comp1",
                    Address = "The Road 1",
                    PostalCode = "1459",
                    PostalPlace = "Oslo",
                    Country = "NO",
                    OrganizationNumber = "911111111"
                },
            PrivateSubscription =
                new PrivateSubscriptionDTO
                {
                    OperatorName = "TELEFONIA",
                    FirstName = "Ola",
                    LastName = "Nordmann",
                    Address = "Hjemmeveien 1",
                    PostalCode = "1234",
                    PostalPlace = "HEIMSTADEN",
                    Country = "NO",
                    BirthDate = new DateTime(1971, 10, 21),
                    Email = "me@example.com"
                },
            MobileNumber = "+4791111111",
            SubscriptionProductId = _subscriptionProductId,
            DataPackage = "20GB",
            AddOnProducts = new List<string> { "FKL" },
            NewOperatorAccount = new NewOperatorAccountRequestedDTO { NewOperatorAccountOwner = "91700000" },
            CustomerReferenceFields = new List<NewCustomerReferenceValue>
            {
                new() { Name = "URef1", Type = "User", CallerId = _callerId, Value = "VAL1"},
                new() { Name = "URef2", Type = "User", CallerId = _callerId, Value = "VAL2"},
                new() { Name = "AccURef1", Type = "Account", CallerId = _callerId, Value = "VAL3"}
            },
            OperatorAccountId = _operatorAccountId,
            OrderExecutionDate = DateTime.UtcNow.AddDays(7),
            SIMCardAction = "NEW",
            SIMCardNumber = "89722020101228153578",
            CallerId = _callerId
        };

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newTransferToBusinessFromPrivate));
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-business", newTransferToBusinessFromPrivate);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostTransferToPrivateSubscriptionOrder_ReturnsCreated()
    {
        var newTransferFromPrivate = new TransferToPrivateSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                OperatorName = "TELEFONIA",
                FirstName = "Ola",
                LastName = "Nordmann",
                Address = "Hjemmeveien 1",
                PostalCode = "1234",
                PostalPlace = "HEIMSTADEN",
                Country = "NO",
                BirthDate = new DateTime(1971, 10, 21),
                Email = "me@example.com"
            },
            MobileNumber = "+4791111111",
            NewSubscription = "Another subscription",
            OperatorName = "Telenor",
            OrderExecutionDate = DateTime.UtcNow.AddDays(20),
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newTransferFromPrivate));
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-private", newTransferFromPrivate);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostChangeSubscriptionOrder_ReturnsCreated()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+47 41414141",
            OperatorName = "Telia - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            SubscriptionOwner = "Ola Normann",
            CallerId = Guid.NewGuid()
        };

        var response =
            await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription",
                postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostChangeSubscriptionOrder_ReturnsBadRequest_WhenMobileNumberIsNull()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            OperatorName = "Telenor - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            SubscriptionOwner = "Ola Normann",
            CallerId = Guid.NewGuid()
        };

        var response =
            await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription",
                postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCancelSubscription_ReturnsCreated()
    {
        var cancelSubscriptionOrder = new NewCancelSubscriptionOrder()
        {
            MobileNumber = "+4791111111",
            DateOfTermination = DateTime.UtcNow.AddDays(5),
            OperatorId = _operatorId,
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(cancelSubscriptionOrder));
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancelSubscriptionOrder);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostOrderSim_ReturnsCreated()
    {
        var newOrderSimSubscriptionOrder = new NewOrderSimSubscriptionOrder()
        {
            SendToName = "My Name",
            Address = new Address(){Street = "No name", Postcode = "1111", City = "Oslo", Country = "Norway"},
            OperatorId = _operatorId,
            Quantity = 2,
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newOrderSimSubscriptionOrder));
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/order-sim", newOrderSimSubscriptionOrder);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostChangeSubscriptionOrder_ReturnsCreated_WhenSubscriptionOwnerIsNull()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+47 41414141",
            OperatorName = "Telia - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            CallerId = Guid.NewGuid()
        };

        var response =
            await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription",
                postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_ReturnsBadRequest_WhenNumberIsNotValid()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+47041414141",
            OperatorName = "Telia - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Phone number +47041414141 not valid for countrycode nb.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_ReturnsBadRequest_EmptyString()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = " ",
            OperatorName = "Telia - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Phone number   not valid for countrycode nb.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_BadReuest_CustomerDoesNotHaveOperatorInSettings()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+4741730873",
            OperatorName = "Telenor - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "20GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Customer does not have this operator Telenor - NO as a setting",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_BadReuest_CustomerDoesNotHaveProductInSettings()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+4741730873",
            OperatorName = "Telia - NO",
            ProductName = "lol",
            PackageName = "20GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Customer does not have product lol as a setting",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_BadRequest_CustomerDoesNotHaveDatapackageForProduct()
    {
        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+4741730873",
            OperatorName = "Telia - NO",
            ProductName = "TOTAL BEDRIFT",
            PackageName = "2GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Customer does not have datapackage 2GB with product TOTAL BEDRIFT as a setting",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostChangeSubscriptionOrder_BadRequest_ProductDontHaveDataPackages()
    {
        var customerSubscriptionProduct = new NewSubscriptionProduct
        {
            Name = "Prod",
            OperatorId = _operatorId,
            DataPackages = null,
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(customerSubscriptionProduct));
        var createResponse = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products", customerSubscriptionProduct);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var postRequest = new NewChangeSubscriptionOrder
        {
            MobileNumber = "+4741730873",
            OperatorName = "Telia - NO",
            ProductName = "Prod",
            PackageName = "2GB",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/change-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Customer does not have datapackage 2GB with product Prod as a setting",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsCreated()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+4741730800",
            OperatorId = 1,
            SimCardNumber = "89652021000371234219",
            SimCardType = "Regular",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsBadRequest_InvalidSimNumber()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+4741730800",
            OperatorId = 1,
            SimCardNumber = "89652021000",
            SimCardType = "Regular",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
           $"SIM card number: 89652021000 not valid.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsBadRequest_InvalidOperator()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+4741730800",
            OperatorId = 5,
            SimCardNumber = "89652021000371234219",
            SimCardType = "Regular",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
             $"No operator with OperatorId 5 found.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsBadRequest_InvalidSimType()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+4741730800",
            OperatorId = 1,
            SimCardNumber = "89652021000371234219",
            SimCardType = "RegularSIM",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
             $"SIM card type: RegularSIM not valid.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsBadRequest_InvalidPhoneNumber()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+47417308",
            OperatorId = 1,
            SimCardNumber = "89652021000371234219",
            SimCardType = "Regular",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
             $"Phone number +47417308 not valid for countrycode nb.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostActivateSim_ReturnsBadRequest_InvalidPhoneNumberWithOperatorCountry()
    {
        var postRequest = new NewActivateSimOrder
        {
            MobileNumber = "+4741730800",
            OperatorId = 2,
            SimCardNumber = "89652021000371234219",
            SimCardType = "Regular",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
             $"Phone number +4741730800 not valid for countrycode se.",
            response.Content.ReadAsStringAsync().Result);
    }



}