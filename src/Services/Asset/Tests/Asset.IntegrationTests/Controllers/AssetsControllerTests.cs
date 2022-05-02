using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Asset.API;
using Asset.API.ViewModels;
using Asset.IntegrationTests.Helpers;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using static System.StringComparison;
using AssetLifecycleType = Asset.API.ViewModels.AssetLifecycleType;
using Label = Asset.API.ViewModels.Label;
using LifeCycleSetting = Asset.API.ViewModels.LifeCycleSetting;

namespace Asset.IntegrationTests.Controllers;

public class AssetsControllerTests : IClassFixture<AssetWebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;
    private readonly Guid _customerId;
    private readonly Guid _departmentId;
    private readonly Guid _user;
    private readonly Guid _assetOne;
    private readonly Guid _assetTwo;

    private readonly AssetWebApplicationFactory<Startup> _factory;

    public AssetsControllerTests(AssetWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _organizationId = factory.ORGANIZATION_ID;
        _customerId = factory.COMPANY_ID;
        _departmentId = factory.DEPARTMENT_ID;
        _user = factory.ASSETHOLDER_ONE_ID;
        _assetOne = factory.ASSETLIFECYCLE_ONE_ID;
        _assetTwo = factory.ASSETLIFECYCLE_TWO_ID;
        _factory = factory;
    }

    [Fact]
    public async Task GetAssetsForCustomer()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(5, pagedAssetList!.Items.Count);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList!.Items[0]!.CreatedDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList!.Items[1]!.CreatedDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList!.Items[2]!.CreatedDate.Date);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomer()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithDepartment()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithStatus()
    {
        var requestUri =
            $"/api/v1/Assets/customers/{_customerId}/count?departmentId=&assetLifecycleStatus={(int)AssetLifecycleStatus.Available}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithDepartmentAndStatus()
    {
        var requestUri =
            $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}&assetLifecycleStatus={(int)AssetLifecycleStatus.Available}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateAssetAndRetrieveAssetsForCustomer()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        const bool IS_PERSONAL = false;
        var managedByDepartmentId = Guid.NewGuid();
        const string ALIAS = "Just another name";
        const int ASSET_CATEGORY_ID = 1;
        const string NOTE = "A long note";
        const string BRAND = "iPhone";
        const string PRODUCT_NAME = "12 Pro Max";
        var purchaseDate = new DateTime(2022, 2, 2);
        var assetHolderId = Guid.NewGuid();
        const string DESCRIPTION = "A long description";
        const long FIRST_IMEI = 356728115537645;
        const decimal PAID_BY_COMPANY = 120m;
        const string ORDER_NUMBER = "ORDER_123";
        const string PRODUCT_ID = "PROD_12345";
        const decimal MONTHLY_SALARY_DEDUCTION = 11m;
        const string USER_EMAIL = "me@not.me";
        const int MONTHLY_SALARY_DEDUCTION_RUNTIME = 24;
        const string USER_PHONE_NUMBER = "+4791111111";
        const string USER_FIRST_NAME = "Jane";
        const string USER_LAST_NAME = "Doe";
        const string INVOICE_NUMBER = "INV44333333343";
        const string TRANSACTION_ID = "43433434542332423242ddde3232";
        var newAsset = new NewAsset
        {
            Alias = ALIAS,
            AssetCategoryId = ASSET_CATEGORY_ID,
            Note = NOTE,
            Brand = BRAND,
            ProductName = PRODUCT_NAME,
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = purchaseDate,
            ManagedByDepartmentId = managedByDepartmentId,
            AssetHolderId = assetHolderId,
            Description = DESCRIPTION,
            Imei = new List<long> { FIRST_IMEI },
            CallerId = _callerId,
            PaidByCompany = PAID_BY_COMPANY,
            OrderNumber = ORDER_NUMBER,
            ProductId = PRODUCT_ID,
            MonthlySalaryDeduction = MONTHLY_SALARY_DEDUCTION,
            MonthlySalaryDeductionRuntime = MONTHLY_SALARY_DEDUCTION_RUNTIME,
            UserEmail = USER_EMAIL,
            UserPhoneNumber = USER_PHONE_NUMBER,
            UserFirstName = USER_FIRST_NAME,
            UserLastName = USER_LAST_NAME,
            InvoiceNumber = INVOICE_NUMBER,
            TransactionId = TRANSACTION_ID,
            IsPersonal = IS_PERSONAL
        };
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";

        // Act
        var createResponse = await httpClient.PostAsJsonAsync(requestUri, newAsset);

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        Assert.Equal(1, pagedAssetList!.Items.Count);
        Assert.Equal(ALIAS, pagedAssetList.Items[0].Alias);
        Assert.Equal(ASSET_CATEGORY_ID, pagedAssetList.Items[0].AssetCategoryId);
        Assert.Equal(NOTE, pagedAssetList.Items[0].Note);
        Assert.Equal(BRAND, pagedAssetList.Items[0].Brand);
        Assert.Equal(PRODUCT_NAME, pagedAssetList.Items[0].ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), pagedAssetList.Items[0].LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), pagedAssetList.Items[0].PurchaseDate.ToShortDateString());
        Assert.Equal(assetHolderId, pagedAssetList.Items[0].AssetHolderId);
        Assert.Equal(managedByDepartmentId, pagedAssetList.Items[0].ManagedByDepartmentId);
        Assert.Equal(FIRST_IMEI, pagedAssetList.Items[0].Imei.FirstOrDefault());
        Assert.Equal(ORDER_NUMBER, pagedAssetList.Items[0].OrderNumber);
        Assert.Equal(PRODUCT_ID, pagedAssetList.Items[0].ProductId);
        Assert.Equal(INVOICE_NUMBER, pagedAssetList.Items[0].InvoiceNumber);
        Assert.Equal(TRANSACTION_ID, pagedAssetList.Items[0].TransactionId);

        requestUri = $"/api/v1/Assets/{pagedAssetList.Items[0].Id}/customers/{_organizationId}";
        var assetLifecycle = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUri);
        Assert.Equal(ALIAS, assetLifecycle!.Alias);
        Assert.Equal(ASSET_CATEGORY_ID, assetLifecycle!.AssetCategoryId);
        Assert.Equal("Mobile phone", assetLifecycle!.AssetCategoryName);
        Assert.Equal(NOTE, assetLifecycle.Note);
        Assert.Equal(BRAND, assetLifecycle.Brand);
        Assert.Equal(PRODUCT_NAME, assetLifecycle.ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), assetLifecycle.LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), assetLifecycle.PurchaseDate.ToShortDateString());
        Assert.Equal(assetHolderId, assetLifecycle.AssetHolderId);
        Assert.Equal(managedByDepartmentId, assetLifecycle.ManagedByDepartmentId);
        Assert.Equal(FIRST_IMEI, assetLifecycle.Imei.FirstOrDefault());
        Assert.Equal(DESCRIPTION, assetLifecycle.Description);
        Assert.Equal(IS_PERSONAL, assetLifecycle.IsPersonal);
    }

    [Fact]
    public async Task CheckLifecyclesReturned()
    {
        var requestUri = "/api/v1/Assets/lifecycles";
        var lifecycles = await _httpClient.GetFromJsonAsync<IList<AssetLifecycleType>>(requestUri);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(lifecycles));
        Assert.Equal(2, lifecycles!.Count);
        Assert.Equal("Transactional", lifecycles.FirstOrDefault(l => l.EnumValue == 2)!.Name);
    }

    [Fact]
    public async Task CreateAssetWithEmptyDescription()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetCategoryId = 1,
            Note = "A long note",
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2022, 2, 2),
            ManagedByDepartmentId = Guid.NewGuid(),
            AssetHolderId = Guid.NewGuid(),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        var message = await createResponse.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(message);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task CreateAssetWithEmptyNote()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetCategoryId = 1,
            Description = "A long description",
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2022, 2, 2),
            ManagedByDepartmentId = Guid.NewGuid(),
            AssetHolderId = Guid.NewGuid(),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task CreateAssetWithoutOwner()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetCategoryId = 1,
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2022, 2, 2),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task UnAssignAssetsFromUser()
    {
        var data = new UnAssignAssetToUser { CallerId = _callerId, DepartmentId = _departmentId };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(data));
        var userId = Guid.Parse("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        var customerId = Guid.Parse("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");

        var requestUri = $"/api/v1/Assets/customers/{customerId}/users/{userId}";
        _testOutputHelper.WriteLine(requestUri);
        var deleteResponse = await _httpClient.PatchAsync(requestUri, JsonContent.Create(data));
        Assert.Equal(HttpStatusCode.Accepted, deleteResponse.StatusCode);

        var pagedAssetList =
            await _httpClient.GetFromJsonAsync<PagedAssetList>($"/api/v1/Assets/customers/{customerId}");

        Assert.NotNull(pagedAssetList);
        Assert.Equal(5, pagedAssetList!.TotalItems);
        Assert.All(pagedAssetList.Items, m => Assert.Equal(data.DepartmentId, m.ManagedByDepartmentId));
        Assert.All(pagedAssetList.Items, m => Assert.Null(m.AssetHolderId));
    }

    [Fact]
    public async Task GetLifeCycleSetting()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var setting = await _httpClient.GetFromJsonAsync<LifeCycleSetting>(requestUri);
        Assert.Equal(setting!.CustomerId, _customerId);
        Assert.True(setting!.BuyoutAllowed);
    }

    [Fact]
    public async Task CreateLifeCycleSetting()
    {
        var newSettings = new NewLifeCycleSetting { BuyoutAllowed = true, CallerId = _callerId };
        var customerId = Guid.NewGuid();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);

        var setting =
            await _httpClient.GetFromJsonAsync<LifeCycleSetting>(
                $"/api/v1/Assets/customers/{customerId}/lifecycle-setting");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(setting!.CustomerId, customerId);
        Assert.True(setting!.BuyoutAllowed == newSettings.BuyoutAllowed);
    }

    [Fact]
    public async Task GetAvailableAssetsForCustomer()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?page=1&limit=1000&status=3";
        _testOutputHelper.WriteLine(requestUri);
        var pagedAsset = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        Assert.True(pagedAsset!.TotalItems == 3);
        Assert.True(pagedAsset!.Items.Count == 3);
        Assert.True(pagedAsset!.Items.Where(x => x.AssetStatus == AssetLifecycleStatus.Available).Count() == 3);
    }


    [Fact]
    public async Task UpdateLifeCycleSetting()
    {
        var newSettings = new NewLifeCycleSetting { BuyoutAllowed = false, CallerId = _callerId };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PutAsJsonAsync(requestUri, newSettings);
        var setting =
            await _httpClient.GetFromJsonAsync<LifeCycleSetting>(
                $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");
        Assert.Equal(setting!.CustomerId, _customerId);
        Assert.True(setting!.BuyoutAllowed == newSettings.BuyoutAllowed);
    }

    [Fact]
    public async Task SetCategoryLifeCycleSetting()
    {
        var newSettings = new NewCategoryLifeCycleSetting
        {
            AssetCategoryId = 1, MinBuyoutPrice = 200m, CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/category-lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);
        var setting =
            await _httpClient.GetFromJsonAsync<LifeCycleSetting>(
                $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");
        Assert.Equal(setting!.CustomerId, _customerId);
        Assert.True(setting!.BuyoutAllowed);
        Assert.True(
            setting!.CategoryLifeCycleSettings.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!
                .MinBuyoutPrice == newSettings.MinBuyoutPrice);
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("NO", null)]
    [InlineData("NO", 1)]
    [InlineData(null, 1)]
    public async Task GetBaseMinBuyoutPrice(string? country, int? assetCategoryId)
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                using var assetsContext = scope.ServiceProvider.GetRequiredService<AssetsContext>();
                var logger = scope.ServiceProvider
                    .GetRequiredService<ILogger<AssetWebApplicationFactory<AssetsControllerTests>>>();

                try
                {
                    AssetTestDataSeedingForDatabase.ResetDbForTests(assetsContext);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception,
                        "An error occurred seeding the " + "database with test data. Error: {Message}",
                        exception.Message);
                }
            });
        }).CreateClient();

        // Act
        var requestUri =
            $"/api/v1/Assets/min-buyout-price?{(string.IsNullOrWhiteSpace(country) ? "" : $"country={country}")}&{(assetCategoryId == null ? "" : $"assetCategoryId={assetCategoryId}")}";
        _testOutputHelper.WriteLine(requestUri);
        var buyoutPrices = await _httpClient.GetFromJsonAsync<IList<MinBuyoutPriceBaseline>>(requestUri);

        if (!string.IsNullOrEmpty(country))
        {
            Assert.True(buyoutPrices!.All(x =>
                string.Equals(x.Country, country, CurrentCultureIgnoreCase)));
        }

        if (assetCategoryId != null)
        {
            Assert.True(buyoutPrices!.All(x => x.AssetCategoryId == assetCategoryId));
        }

        if (string.IsNullOrEmpty(country) && assetCategoryId == null)
        {
            Assert.Equal(2, buyoutPrices!.Count);
        }
    }

    [Fact]
    public async Task AssignLabel_AddedLabelToAsset()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var labels = new List<NewLabel>
        {
            new() { Color = LabelColor.Blue, Text = "Label1" }, new() { Color = LabelColor.Green, Text = "Label2" }
        };

        var data = new AddLabelsData { CallerId = _callerId, NewLabels = labels };

        //Post label
        var requestUriPost = $"/api/v1/Assets/customers/{_customerId}/labels";
        var responsePost = await httpClient.PostAsync(requestUriPost, JsonContent.Create(data));
        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);

        var labelsList = await responsePost.Content.ReadFromJsonAsync<IList<Label>>();
        Assert.Equal(3, labelsList!.Count);
        Assert.Contains(labelsList!, l => string.Equals(l.Text, "Label1", InvariantCulture));

        var labelGuid = new List<Guid> { labelsList![0].Id, labelsList[1].Id };
        var assetGuid = new List<Guid> { _assetOne };

        //Assign label
        var requestUriAssign = $"/api/v1/Assets/customers/{_customerId}/labels/assign";
        var assetLabel = new AssetLabels { AssetGuids = assetGuid, LabelGuids = labelGuid, CallerId = _callerId };

        var responseAssign = await httpClient.PostAsync(requestUriAssign, JsonContent.Create(assetLabel));
        Assert.Equal(HttpStatusCode.OK, responseAssign.StatusCode);

        //Get asset with label
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";

        var asset = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUri);
        Assert.NotNull(asset);
        Assert.Equal(2, asset?.Labels.Count);

        //Fetch all assets for customer
        var requestAllAssets = $"/api/v1/Assets/customers/{_customerId}";
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestAllAssets);
        Assert.Equal(5, pagedAssetList!.Items.Count);
        var assetOneFromResponse = pagedAssetList.Items.FirstOrDefault(i => i.Id == _assetOne);
        Assert.Equal(_assetOne, assetOneFromResponse!.Id);
        Assert.Equal(2, assetOneFromResponse.Labels.Count);
    }
    [Fact]
    public async Task GetLabels_CheckViewModel()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/labels";

        var labels = await _httpClient.GetFromJsonAsync<List<API.ViewModels.Label>>(requestUri);
        Assert.NotNull(labels);
        Assert.Equal(1, labels?.Count);
        Assert.Equal("CompanyOne", labels[0]?.Text);
        Assert.Equal("Lightblue", labels[0]?.ColorName);
        Assert.NotNull(labels[0]?.Id);
        Assert.NotEqual(Guid.Empty,labels[0]?.Id);
    }





}