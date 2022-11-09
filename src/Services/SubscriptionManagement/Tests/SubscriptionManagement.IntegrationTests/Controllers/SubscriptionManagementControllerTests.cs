using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Enums;
using Common.Interfaces;
using SubscriptionManagement.API.Controllers;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace SubscriptionManagement.IntegrationTests.Controllers;
public class MockDateTimeProvider : IDateTimeProvider
{
    public DateTime GetNow() => new DateTime(2022, 05, 02);
}
public class
    SubscriptionManagementControllerTests : IClassFixture<
        SubscriptionManagementWebApplicationFactory<SubscriptionManagementController>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;
    private readonly string _phoneNumber;
    private readonly int _operatorId;
    private readonly int _operatorAccountId;
    private readonly int _subscriptionProductId;
    private readonly MockDateTimeProvider _mockDateTime = new MockDateTimeProvider();


    public SubscriptionManagementControllerTests(
        SubscriptionManagementWebApplicationFactory<SubscriptionManagementController> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _subscriptionProductId = factory.CUSTOMER_SUBSCRIPTION_PRODUCT_ID;
        _organizationId = factory.ORGANIZATION_ID;
        _phoneNumber = factory.PHONE_NUMBER;
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
            p.Name == customerSubscriptionProduct.Name && !p.IsGlobal);
        var deleteResponse = await _httpClient.DeleteAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products/{retrievedNewCustomerSubscriptionProduct!.Id}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task GetOperatorSubscriptionProductForCustomer()
    {
        var modelList =
            await _httpClient.GetFromJsonAsync<IList<CustomerSubscriptionProductDTO>>(
                $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products?includeOperator=true");

        Assert.True(modelList!.Count > 0);
        Assert.True(modelList.FirstOrDefault()!.OperatorId != 0);
    }


    [Fact]
    public async Task CreateAndUpdateCustomerSubscriptionProduct()
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

        var newCustomerSubscriptionProduct = await createResponse.Content.ReadFromJsonAsync<CustomerSubscriptionProductDTO>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal("A customer subscription product", newCustomerSubscriptionProduct!.Name);
        Assert.Equal(1, newCustomerSubscriptionProduct!.DataPackages.Count);

        var updatedSubscriptionProduct = new UpdatedSubscriptionProduct
        {
            DataPackages = new List<string> { "25GB" },
            Name = "A customer subscription product"
        };

        var patchResponse = await _httpClient.PatchAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-products/{newCustomerSubscriptionProduct.Id}", JsonContent.Create(updatedSubscriptionProduct));
        
        var customerSubscriptionProductDTO = await patchResponse.Content.ReadFromJsonAsync<CustomerSubscriptionProductDTO>();
        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        Assert.Equal(newCustomerSubscriptionProduct.Name, customerSubscriptionProductDTO!.Name);
        Assert.Equal(new List<string> { "20GB" , "25GB" }, customerSubscriptionProductDTO!.DataPackages);
    }
    [Fact]
    public async Task GetSubscriptionOrdersCountAsync_ForCustomer()
    {
        var response = await _httpClient.GetAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/count");
        var resp = await response.Content.ReadAsStringAsync();
        int.TryParse(resp, out var count);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10, count);
    }
    [Fact]
    public async Task GetSubscriptionOrdersCountAsync_ByPhonerNumber()
    {
        var response = await _httpClient.GetAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/count?phoneNumber={_phoneNumber}");
        var resp = await response.Content.ReadAsStringAsync();
        int.TryParse(resp, out var count);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, count);
    }
    [Fact]
    public async Task CreateAndCheckOperatorAccountName40Chars()
    {
        var newOperatorAccount = new NewOperatorAccount()
        {
            AccountNumber = "12343",
            AccountName = "123456789012345678901234567890123456789012345678901234567890",
            OperatorId = _operatorId,
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newOperatorAccount));
        var createResponse = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/operator-accounts", newOperatorAccount);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var operatorAccounts = await _httpClient.GetFromJsonAsync<IList<CustomerOperatorAccount>>($"/api/v1/SubscriptionManagement/{_organizationId}/operator-accounts");
        var operatorAccount = operatorAccounts!.FirstOrDefault(o => o.AccountNumber == "12343");

        Assert.Equal("1234567890123456789012345678901234567890", operatorAccount!.AccountName);
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
                    OperatorName = "Telia - NO",
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
            OperatorId = 1,
            OperatorAccountId = _operatorAccountId,
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(7),
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
    public async Task PostTransferPrivateToBusinessSubscriptionOrder_WithNewOperatorAccount_ReturnsCreated()
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
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            NewOperatorAccount = new NewOperatorAccountRequestedDTO { NewOperatorAccountOwner = "", OperatorId = _operatorId, NewOperatorAccountPayer = "me" },
            CustomerReferenceFields = new List<NewCustomerReferenceValue>
            {
                new() { Name = "URef1", Type = "User", CallerId = _callerId, Value = "VAL1"},
                new() { Name = "URef2", Type = "User", CallerId = _callerId, Value = "VAL2"},
                new() { Name = "AccURef1", Type = "Account", CallerId = _callerId, Value = "VAL3"}
            },
            OperatorId = 1,
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(7),
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
    public async void PostTransferToPrivateSubscriptionOrder_ReturnsCreated_WhenPrivateSubscription()
    {

        var request = new TransferToPrivateSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionResponse
            {
                OperatorName = "Telia - NO",
                FirstName = "Kari",
                LastName = "Nordmann",
                Address = "Addresse 1",
                PostalCode = "1234",
                PostalPlace = "Oslo",
                Country = "NO",
                BirthDate = new DateTime(1971, 10, 21),
                Email = "me@example.com"
            },
            MobileNumber = "+4745454545",
            NewSubscription= "PrivateSubscription",
            OperatorName = "Telia - NO",
            OrderExecutionDate= _mockDateTime.GetNow().AddDays(21),
            CallerId= Guid.NewGuid()
        };
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-private", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task PostTransferToPrivateSubscriptionOrder_ReturnsBadRequest_CustomerSettingsOperatorDoesNotHaveSubscriptionProduct()
    {
        var newTransferFromPrivate = new TransferToPrivateSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionResponse
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
            NewSubscription = "Not A Product",
            OperatorName = "Telia - NO",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(21),
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newTransferFromPrivate));
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-private", newTransferFromPrivate);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task PostTransferToPrivateSubscriptionOrder_ReturnsBadRequest_InvalidPhoneNumber()
    {
        var newTransferFromPrivate = new TransferToPrivateSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionResponse
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
            MobileNumber = "+47450045",
            NewSubscription = "TOTAL BEDRIFT",
            OperatorName = "Telia - NO",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(21),
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newTransferFromPrivate));
        var response = await _httpClient.PostAsJsonAsync(
            $"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-private", newTransferFromPrivate);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            DateOfTermination = _mockDateTime.GetNow().AddDays(3),
            OperatorId = _operatorId,
            CallerId = Guid.NewGuid()
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(cancelSubscriptionOrder));
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancelSubscriptionOrder);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task PostCancelSubscription_ShouldReturnCreate()
    {
        var cancelSubscriptionOrder = new NewCancelSubscriptionOrder()
        {
            MobileNumber = "+4791111111",
            DateOfTermination = _mockDateTime.GetNow().AddDays(3),
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
            Address = new Address() { Street = "No name", Postcode = "1111", City = "Oslo", Country = "NO" },
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
        Assert.Contains(
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
        Assert.Contains(
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
        Assert.Contains(
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
        Assert.Contains(
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
        Assert.Contains(
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
        Assert.Contains(
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
            SimCardType = "Main",
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
        Assert.Contains(
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
        Assert.Contains(
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
        Assert.Contains(
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
            SimCardType = "Main",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
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
            SimCardType = "Main",
            CallerId = Guid.NewGuid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/activate-sim", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
             $"Phone number +4741730800 not valid for countrycode se.",
            response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task PostTransferPrivateToBusinessSubscriptionOrder_ReturnCreated_AllowingNullForProductDataPackages()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new TransferToBusinessSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "NO",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            MobileNumber = "+4747474744",
            OperatorId = 1,
            OperatorAccountId = 100,
            SubscriptionProductId = 200,
            DataPackage = null,
            SIMCardNumber = "89202051293671023971",
            SIMCardAction = "New",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(7),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            BusinessSubscription = new BusinessSubscriptionDTO
            {
                Name = "D&D METAL",
                OrganizationNumber = "995053179",
                Address = "Josipa Kozarca BB",
                PostalCode = "9090",
                PostalPlace = "HR-31207 OSIJEK-TENJA",
                Country = "HR",
                OperatorName = "Telia - NO"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-business", postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    }
    [Fact]
    public async Task PostTransferPrivateToBusinessSubscriptionOrder_ReturnCreated_NullForSimWhenSimCardActionIsOrder()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new TransferToBusinessSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "NO",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            MobileNumber = "+4747474744",
            OperatorAccountId = 100,
            OperatorId = 1,
            SubscriptionProductId = 200,
            DataPackage = null,
            SIMCardNumber = null,
            SIMCardAction = "Order",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(7),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            BusinessSubscription = new BusinessSubscriptionDTO
            {
                Name = "D&D METAL",
                OrganizationNumber = "995053179",
                Address = "Josipa Kozarca BB",
                PostalCode = "9090",
                PostalPlace = "HR-31207 OSIJEK-TENJA",
                Country = "HR",
                OperatorName = "Telia - NO"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-business", postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    }
    [Fact]
    public async Task PostTransferPrivateToBusinessSubscriptionOrder_ReturnBadRequest_ActionIsNewAndNoSimCardNumber()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new TransferToBusinessSubscriptionOrderDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "M@H.COM",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            MobileNumber = "+4747474744",
            OperatorAccountId = 100,
            SubscriptionProductId = 200,
            DataPackage = null,
            SIMCardNumber = null,
            SIMCardAction = "NEW",
            OrderExecutionDate = DateTime.Parse("2022-03-17T00:00:00.000Z"),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            BusinessSubscription = new BusinessSubscriptionDTO
            {
                Name = "D&D METAL",
                OrganizationNumber = "995053179",
                Address = "Josipa Kozarca BB",
                PostalCode = "9090",
                PostalPlace = "HR-31207 OSIJEK-TENJA",
                Country = "HR",
                OperatorName = "Telia - NO"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-business", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }
    [Fact]
    public async Task PostNewSubscriptionOrder_ReturnCreated()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new NewSubscriptionOrderRequestDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "NO",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            OperatorId = 1,
            OperatorAccountId = 100,
            SubscriptionProductId = 200,
            SimCardNumber = "89202051293671023971",
            SimCardAction = "NEW",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(7),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            BusinessSubscription = new BusinessSubscriptionDTO
            {
                Name = "D&D METAL",
                OrganizationNumber = "995053179",
                Address = "Josipa Kozarca BB",
                PostalCode = "9090",
                PostalPlace = "HR-31207 OSIJEK-TENJA",
                Country = "HR",
                OperatorName = "Telia - NO"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/new-subscription", postRequest);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task PostNewSubscriptionOrder_ReturnBadRequest_ActionNewSimAndSimCardNumber()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new NewSubscriptionOrderRequestDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "M@H.COM",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            OperatorId = 1,
            OperatorAccountId = 100,
            SubscriptionProductId = 200,
            SimCardAction = "NEW",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(3),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" },
            BusinessSubscription = new BusinessSubscriptionDTO
            {
                Name = "D&D METAL",
                OrganizationNumber = "995053179",
                Address = "Josipa Kozarca BB",
                PostalCode = "9090",
                PostalPlace = "HR-31207 OSIJEK-TENJA",
                Country = "HR",
                OperatorName = "Telia - NO"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/new-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task PostNewSubscriptionOrder_ReturnBadRequest_ActionOrderSimAndNoAddress()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new NewSubscriptionOrderRequestDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "M@H.COM",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            OperatorId = 1,
            OperatorAccountId = 100,
            SubscriptionProductId = 200,
            SimCardAction = "Order",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(3),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/new-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task PostNewSubscriptionOrder_ReturnBadRequest_NoOperatorAccountIdAndNoNewOperatorAccount()
    {
        var referenceFields = new NewCustomerReferenceValue
        {
            Name = "URef1",
            Type = "User",
            Value = "dfghjkl",
        };


        var postRequest = new NewSubscriptionOrderRequestDTO
        {
            PrivateSubscription = new PrivateSubscriptionDTO
            {
                FirstName = "Ole",
                LastName = "Brum",
                Address = "K. K. Lien vei 54F",
                PostalCode = "4812",
                PostalPlace = "KONGSHAVN",
                Country = "M@H.COM",
                Email = "me@example.com",
                BirthDate = DateTime.Parse("1986-06-04T00:00:00.000Z"),
                OperatorName = "Telia - NO"
            },
            OperatorId = 1,
            OperatorAccountId = 0,
            SubscriptionProductId = 200,
            SimCardAction = "Order",
            OrderExecutionDate = _mockDateTime.GetNow().AddDays(3),
            CustomerReferenceFields = new List<NewCustomerReferenceValue> { referenceFields },
            AddOnProducts = new List<string> { "InvoiceControl", "CorporateNetwork" }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/new-subscription", postRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostStandardPrivateSubscriptionProducts_ReturnsCreated()
    {
        var standardProduct = new NewCustomerStandardPrivateSubscriptionProduct
        {
            OperatorId = 1,
            SubscriptionName = "Subscription",
            DataPackage = "7GB",
            CallerId = new Guid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-private-subscription-products", standardProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task GetStandardPrivateSubscriptionProducts_ReturnsOK()
    {
        var standardProduct = new NewCustomerStandardPrivateSubscriptionProduct
        {
            OperatorId = 1,
            SubscriptionName = "Subscription",
            DataPackage = "7GB",
            CallerId = new Guid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-private-subscription-products", standardProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseGet = await _httpClient.GetAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-private-subscription-products");
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
        var read = await responseGet.Content.ReadFromJsonAsync<IList<SubscriptionManagementServices.Models.CustomerStandardPrivateSubscriptionProduct>>();
        Assert.Equal(1, read?.Count);
    }
    [Fact]
    public async Task DeleteStandardPrivateSubscriptionProducts_ReturnsOK()
    {
        var standardProduct = new NewCustomerStandardPrivateSubscriptionProduct
        {
            OperatorId = 1,
            SubscriptionName = "Subscription",
            DataPackage = "7GB",
            CallerId = new Guid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-private-subscription-products", standardProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var requestUri = $"/api/v1/SubscriptionManagement/{_organizationId}/standard-private-subscription-products/{1}";
        HttpRequestMessage request = new HttpRequestMessage
        {
            Content = JsonContent.Create(_callerId),
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestUri, UriKind.Relative)
        };
        var responseDelete = await _httpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
    }
    [Fact]
    public async Task CancelSubscription_dateOfTermination()
    {
        var cancel = new NewCancelSubscriptionOrder
        {
            OperatorId = 1,
            MobileNumber = "+4741454546",
            CallerId = new Guid()
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancel);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
         $"Invalid transfer date. 2 workday ahead or more is allowed.",
        response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task CancelSubscription_dateOfTermination_CantBeAweekendDayException()
    {
        var cancel = new NewCancelSubscriptionOrder
        {
            OperatorId = 1,
            MobileNumber = "+4741454546",
            CallerId = new Guid(),
            DateOfTermination = _mockDateTime.GetNow().AddMonths(2)
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancel);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
                $"Transfer date can not be a Saturday.",
               response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task CancelSubscription_dateOfTermination_Within30DaysException()
    {
        var cancel = new NewCancelSubscriptionOrder
        {
            OperatorId = 1,
            MobileNumber = "+4741454546",
            CallerId = new Guid(),
            DateOfTermination = _mockDateTime.GetNow().AddMonths(2).AddDays(2)
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancel);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
                $"Invalid date. Needs to be within 30 business days.",
               response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task GetDetailViewSubscriptionOrderLog_ReturnOK()
    {
        
        int orderType = 7;

        var cancel = new NewCancelSubscriptionOrder
        {
            OperatorId = 1,
            DateOfTermination = _mockDateTime.GetNow().AddDays(7),
            MobileNumber = "+4741454546",
            CallerId = new Guid()
        };

        var responseCancel = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancel);
        Assert.Equal(HttpStatusCode.Created, responseCancel.StatusCode);
        
        var subscriptionOrderList = await _httpClient.GetFromJsonAsync<List<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders");
        


        string requestUri = $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders-detail-view/{subscriptionOrderList[0].SubscriptionOrderId}/{orderType}";
        var response = await _httpClient.GetAsync(requestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task GetDetailViewSubscriptionOrderLog_ReturnBadRequestWrongOrderType()
    {

        int orderType = 5;

        var cancel = new NewCancelSubscriptionOrder
        {
            OperatorId = 1,
            DateOfTermination = _mockDateTime.GetNow().AddDays(4),
            MobileNumber = "+4741454546",
            CallerId = new Guid()
        };

        var responseCancel = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-cancel", cancel);
        Assert.Equal(HttpStatusCode.Created, responseCancel.StatusCode);

        var subscriptionOrderList = await _httpClient.GetFromJsonAsync<List<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders");



        string requestUri = $"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders-detail-view/{subscriptionOrderList[0].SubscriptionOrderId}/{orderType}";
        var response = await _httpClient.GetAsync(requestUri);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
        $"Can't find the order with id: {subscriptionOrderList[0].SubscriptionOrderId}",
       response.Content.ReadAsStringAsync().Result);
    }
    [Fact]
    public async Task GetAllSubscriptionOrders()
    {
        // Act
        var response = await _httpClient.GetFromJsonAsync<PagedModel<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/pagination?page={1}&limit={25}");
        
        // Assert
        Assert.Equal(15, response!.TotalItems);
    }
    [Fact]
    public async Task GetAllSubscriptionOrders_ByPhoneNumber()
    {
        // Arrange
        var search = "99999998";

        // Act
        var response = await _httpClient.GetFromJsonAsync<PagedModel<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/pagination?q={search}&page={1}&limit={25}");
        
        // Assert
        Assert.Equal(1, response!.TotalItems);
        Assert.Equal(search, response!.Items[0].PhoneNumber);
    }
    [Fact]
    public async Task GetAllSubscriptionOrders_ByOrderNumber()
    {
        // Arrange
        var orderNumber = "911";

        // Act
        var response = await _httpClient.GetFromJsonAsync<PagedModel<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/pagination?q={orderNumber}&page={1}&limit={25}");

        // Assert
        Assert.Equal(5, response!.TotalItems);
    }

    [Fact]
    public async Task GetAllSubscriptionOrders_ByOrderType()
    {
        // Arrange
        var filterOptions = new FilterOptionsForSubscriptionOrder
        {
            OrderTypes = new List<int>() { (int)SubscriptionOrderTypes.TransferToBusiness }
        };
        var json = JsonSerializer.Serialize(filterOptions);

        // Act
        var response = await _httpClient.GetFromJsonAsync<PagedModel<SubscriptionOrderListItemDTO>>($"/api/v1/SubscriptionManagement/{_organizationId}/subscription-orders/pagination?page={1}&limit={25}&filterOptions={json}");
        
        // Assert
        Assert.Equal(3, response!.TotalItems);
        Assert.Equal((int)SubscriptionOrderTypes.TransferToBusiness, response!.Items[0].OrderTypeId);
    }

    [Fact]
    public async Task DeleteStandardBusinessSubscriptionProducts()
    {
        var response = await _httpClient.DeleteAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-business-subscription-products/{_operatorId}");
        var standardBusinessSubscription = await response.Content.ReadFromJsonAsync<CustomerStandardBusinessSubscriptionProductDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        Assert.NotNull(standardBusinessSubscription);
        Assert.Equal(_operatorId, standardBusinessSubscription.OperatorId);
        Assert.Equal("Telia - NO", standardBusinessSubscription.OperatorName);
        Assert.Equal("BusinessDataPackage", standardBusinessSubscription.DataPackage);
        Assert.Equal(1,standardBusinessSubscription.AddOnProducts!.Count);
    }

    [Fact]
    public async Task AddStandardBusinessSubscriptionProducts()
    {
        // Arrange
        var subscriptionName = "Subscription free";
        var datapackage = "15GB";
        var newCustomerStandardBusinessSubscriptionProduct = new NewCustomerStandardBusinessSubscriptionProduct
        {
            OperatorId = _operatorId,
            DataPackage = datapackage,
            SubscriptionName = subscriptionName,
            AddOnProducts = new List<string> { "Faktura kontroll" },
            CallerId = _callerId
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-business-subscription-products", newCustomerStandardBusinessSubscriptionProduct);
        var standardBusinessSubscription = await response.Content.ReadFromJsonAsync<CustomerStandardBusinessSubscriptionProductDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(standardBusinessSubscription);
        Assert.Equal("Telia - NO", standardBusinessSubscription.OperatorName);
        Assert.Equal(_operatorId, standardBusinessSubscription.OperatorId);
        Assert.Equal(1, standardBusinessSubscription.AddOnProducts!.Count);
        Assert.Equal(subscriptionName, standardBusinessSubscription.SubscriptionName);
        Assert.Equal(datapackage, standardBusinessSubscription.DataPackage);
    }

    [Fact]
    public async Task GetStandardBusinessSubscriptionProducts()
    {
        // Arrange
        var response = await _httpClient.GetAsync($"/api/v1/SubscriptionManagement/{_organizationId}/standard-business-subscription-products");
        var standardBusinessSubscription = await response.Content.ReadFromJsonAsync<List<CustomerStandardBusinessSubscriptionProductDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(standardBusinessSubscription);

        Assert.Collection(standardBusinessSubscription,
           item => Assert.Equal("Telia - NO", item.OperatorName)
       );
        Assert.Collection(standardBusinessSubscription,
            item => Assert.Equal("BusinessSubscription", item.SubscriptionName)
        );
        Assert.Collection(standardBusinessSubscription,
            item => Assert.Equal("BusinessDataPackage", item.DataPackage)
        );
        Assert.Collection(standardBusinessSubscription,
           item => Assert.Equal(1, item.AddOnProducts!.Count)
       );
    }
}
