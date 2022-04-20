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

    public AssetsControllerTests(AssetWebApplicationFactory<AssetsController> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _organizationId = factory.ORGANIZATION_ID;
        _customerId = factory.COMPANY_ID;
    }

    [Fact]
    public async Task GetAssetsForCustomer()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}";
        _testOutputHelper.WriteLine(requestUri);
        PagedAssetList? pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        Assert.Equal(3, pagedAssetList!.Items.Count);
        Assert.Equal(DateTime.Today.Date, pagedAssetList!.Items[0]!.CreatedDate.Date);
        Assert.Equal(DateTime.Today.Date, pagedAssetList!.Items[1]!.CreatedDate.Date);
        Assert.Equal(DateTime.Today.Date, pagedAssetList!.Items[2]!.CreatedDate.Date);
    }

    [Fact]
    public async Task CreateAssetAndRetrieveAssetsForCustomer()
    {
        var managedByDepartmentId = Guid.NewGuid();
        var @alias = "Just another name";
        var assetCategoryId = 1;
        var note = "A long note";
        var brand = "iPhone";
        var productName = "12 Pro Max";
        var purchaseDate = new DateTime(2022, 2, 2);
        var assetHolderId = Guid.NewGuid();
        var description = "A long description";
        var firstImei = 356728115537645;
        var newAsset = new NewAsset
        {
            Alias = @alias,
            AssetCategoryId = assetCategoryId,
            Note = note,
            Brand = brand,
            ProductName = productName,
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = purchaseDate,
            ManagedByDepartmentId = managedByDepartmentId,
            AssetHolderId = assetHolderId,
            Description = description,
            Imei = new List<long> { firstImei },
            CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        PagedAssetList? pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        Assert.Equal(1, pagedAssetList!.Items.Count);
        Assert.Equal(@alias, pagedAssetList.Items[0].Alias);
        Assert.Equal(assetCategoryId, pagedAssetList.Items[0].AssetCategoryId);
        Assert.Equal(note, pagedAssetList.Items[0].Note);
        Assert.Equal(brand, pagedAssetList.Items[0].Brand);
        Assert.Equal(productName, pagedAssetList.Items[0].ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), pagedAssetList.Items[0].LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), pagedAssetList.Items[0].PurchaseDate.ToShortDateString());
        Assert.Equal(assetHolderId, pagedAssetList.Items[0].AssetHolderId);
        Assert.Equal(managedByDepartmentId, pagedAssetList.Items[0].ManagedByDepartmentId);
        Assert.Equal(firstImei, pagedAssetList.Items[0].Imei.FirstOrDefault());
        Assert.Equal(description, pagedAssetList.Items[0].Description);

        requestUri = $"/api/v1/Assets/{pagedAssetList.Items[0].Id}/customers/{_organizationId}";
        var assetLifecycle = await _httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUri);
        Assert.Equal(@alias, assetLifecycle!.Alias);
        Assert.Equal(assetCategoryId, assetLifecycle!.AssetCategoryId);
        Assert.Equal("Mobile phone", assetLifecycle!.AssetCategoryName);
        Assert.Equal(note, assetLifecycle.Note);
        Assert.Equal(brand, assetLifecycle.Brand);
        Assert.Equal(productName, assetLifecycle.ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), assetLifecycle.LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), assetLifecycle.PurchaseDate.ToShortDateString());
        Assert.Equal(assetHolderId, assetLifecycle.AssetHolderId);
        Assert.Equal(managedByDepartmentId, assetLifecycle.ManagedByDepartmentId);
        Assert.Equal(firstImei, assetLifecycle.Imei.FirstOrDefault());
        Assert.Equal(description, assetLifecycle.Description);
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
            DepartmentId = Guid.NewGuid()
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
        Assert.Equal(3, pagedAssetList!.TotalItems);
        Assert.All(pagedAssetList.Items, m => Assert.Equal(data.DepartmentId, m.ManagedByDepartmentId));
        Assert.All(pagedAssetList.Items, m => Assert.Null(m.AssetHolderId));
    }
}