using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SubscriptionManagement.API.Controllers;
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
    private readonly int _operatorAccountId;
    private readonly int _subscriptionProductId;


    public SubscriptionManagementControllerTests(
        SubscriptionManagementWebApplicationFactory<SubscriptionManagementController> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _subscriptionProductId = factory.CUSTOMER_SUBSCRIPTION_PRODUCT_ID;
        _organizationId = factory.ORGANIZATION_ID;
        _operatorAccountId = factory.OPERATOR_ACCOUNT_ID;
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
            CustomerReferenceFields = new List<NewCustomerReferenceField>
            {
                new() { Name = "URef1", Type = "User", CallerId = _callerId },
                new() { Name = "URef2", Type = "User", CallerId = _callerId },
                new() { Name = "AccURef1", Type = "Account", CallerId = _callerId }
            },
            OperatorAccountId = _operatorAccountId,
            OrderExecutionDate = DateTime.UtcNow.AddDays(7),
            SIMCardAction = "NEW",
            SIMCardNumber = "9453875439857349853487",
            CallerId = _callerId
        };

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newTransferToBusinessFromPrivate));
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-business", newTransferToBusinessFromPrivate);

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
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/SubscriptionManagement/{_organizationId}/transfer-to-private", newTransferFromPrivate);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}