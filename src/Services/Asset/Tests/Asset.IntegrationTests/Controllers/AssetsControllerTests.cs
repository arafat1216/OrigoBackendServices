using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Asset.API.Controllers;
using Asset.API.ViewModels;
using Common.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Asset.IntegrationTests.Controllers;

public class AssetsControllerTests : IClassFixture<AssetWebApplicationFactory<AssetsController>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;
    private readonly Guid _customerId;
    private readonly Guid _departmentId;
    private readonly AssetWebApplicationFactory<AssetsController> _factory;

    public AssetsControllerTests(AssetWebApplicationFactory<AssetsController> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _organizationId = factory.ORGANIZATION_ID;
        _customerId = factory.COMPANY_ID;
        _departmentId = factory.DEPARTMENT_ID;
        _factory = factory;
    }

    [Fact]
    public async Task GetAssetsForCustomer()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}";
        _testOutputHelper.WriteLine(requestUri);
        PagedAssetList? pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
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
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count?departmentId=&assetLifecycleStatus={(int)AssetLifecycleStatus.Available}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithDepartmentAndStatus()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}&assetLifecycleStatus={(int)AssetLifecycleStatus.Available}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateAssetAndRetrieveAssetsForCustomer()
    {
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
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        PagedAssetList? pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
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
        var assetLifecycle = await _httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUri);
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
        var requestUri = $"/api/v1/Assets/lifecycles";
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
        var data = new UnAssignAssetToUser
        {
            CallerId = _callerId,
            DepartmentId = _departmentId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(data));
        var userId = Guid.Parse("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        var customerId = Guid.Parse("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");

        var requestUri = $"/api/v1/Assets/customers/{customerId}/users/{userId}";
        _testOutputHelper.WriteLine(requestUri);
        var deleteResponse = await _httpClient.PatchAsync(requestUri, JsonContent.Create(data));
        Assert.Equal(HttpStatusCode.Accepted, deleteResponse.StatusCode);

        var pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>($"/api/v1/Assets/customers/{customerId}");

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
        var newSettings = new NewLifeCycleSetting()
        {
            BuyoutAllowed = true,
            CallerId = _callerId,
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{Guid.NewGuid()}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateLifeCycleSetting()
    {
        var newSettings = new NewLifeCycleSetting()
        {
            BuyoutAllowed = false,
            CallerId = _callerId,
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PutAsJsonAsync(requestUri, newSettings);
        var setting = await _httpClient.GetFromJsonAsync<LifeCycleSetting>($"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");
        Assert.Equal(setting!.CustomerId, _customerId);
        Assert.True(setting!.BuyoutAllowed == newSettings.BuyoutAllowed);
    }

    [Fact]
    public async Task SetCategoryLifeCycleSetting()
    {
        var newSettings = new NewCategoryLifeCycleSetting()
        {
            AssetCategoryId = 1,
            MinBuyoutPrice = 200m,
            CallerId = _callerId,
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/category-lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);
        var setting = await _httpClient.GetFromJsonAsync<LifeCycleSetting>($"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");
        Assert.Equal(setting!.CustomerId, _customerId);
        Assert.True(setting!.BuyoutAllowed);
        Assert.True(setting!.CategoryLifeCycleSettings.FirstOrDefault(x=>x.AssetCategoryId == newSettings.AssetCategoryId)!.MinBuyoutPrice == newSettings.MinBuyoutPrice);
    }






}