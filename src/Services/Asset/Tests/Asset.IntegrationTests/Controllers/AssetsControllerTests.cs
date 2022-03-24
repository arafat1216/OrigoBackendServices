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

public class
    AssetsControllerTests : IClassFixture<
        AssetWebApplicationFactory<AssetsController>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;

    public AssetsControllerTests(
        AssetWebApplicationFactory<AssetsController> factory,
        ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _organizationId = factory.ORGANIZATION_ID;
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
        Assert.Equal(1, pagedAssetList!.Assets.Count);
        Assert.Equal(@alias, pagedAssetList.Assets[0].Alias);
        Assert.Equal(assetCategoryId, pagedAssetList.Assets[0].AssetCategoryId);
        Assert.Equal(note, pagedAssetList.Assets[0].Note);
        Assert.Equal(brand, pagedAssetList.Assets[0].Brand);
        Assert.Equal(productName, pagedAssetList.Assets[0].ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), pagedAssetList.Assets[0].LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), pagedAssetList.Assets[0].PurchaseDate.ToShortDateString());
        Assert.Equal(assetHolderId, pagedAssetList.Assets[0].AssetHolderId);
        Assert.Equal(managedByDepartmentId, pagedAssetList.Assets[0].ManagedByDepartmentId);
        Assert.Equal(firstImei, pagedAssetList.Assets[0].Imei.FirstOrDefault());
        Assert.Equal(description, pagedAssetList.Assets[0].Description);

        requestUri = $"/api/v1/Assets/{pagedAssetList.Assets[0].Id}/customers/{_organizationId}";
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
}