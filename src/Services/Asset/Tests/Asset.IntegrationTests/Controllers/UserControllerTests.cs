using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Asset.API;
using Asset.API.Controllers;
using Asset.API.ViewModels;
using Asset.IntegrationTests.Helpers;
using Common.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Asset.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<AssetWebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;
    private readonly Guid _callerId = Guid.Parse("1d64e718-97cb-11ec-ad86-00155d64bd3d");
    private readonly Guid _organizationId;
    private readonly Guid _customerId;
    private readonly Guid _customerIdTwo;
    private readonly Guid _departmentId;
    private readonly Guid _departmentIdTwo;
    private readonly Guid _user;
    private readonly Guid _assetOne;
    private readonly Guid _assetTwo;
    private readonly Guid _assetThree;
    private readonly Guid _assetFive;
    private readonly Guid _assetEight;


    private readonly AssetWebApplicationFactory<Startup> _factory;

    public UserControllerTests(AssetWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateDefaultClient();
        _organizationId = factory.ORGANIZATION_ID;
        _customerId = factory.COMPANY_ID;
        _departmentId = factory.DEPARTMENT_ID;
        _departmentIdTwo = factory.DEPARTMENT_TWO_ID;
        _user = factory.ASSETHOLDER_ONE_ID;
        _assetOne = factory.ASSETLIFECYCLE_ONE_ID;
        _assetTwo = factory.ASSETLIFECYCLE_TWO_ID;
        _assetThree = factory.ASSETLIFECYCLE_THREE_ID;
        _assetEight = factory.ASSETLIFECYCLE_EIGHT_ID;
        _assetFive = factory.ASSETLIFECYCLE_FIVE_ID;
        _customerIdTwo = factory.COMPANY_ID_TWO;
        _factory = factory;
    }

    [Fact]
    public async Task UserDeleted_CheckAssetLifecycleAssignments()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/assets/user-deleted";
        var deleteUserRequestEvent = new UserDeletedEvent { CustomerId = _customerId, UserId = _user , DepartmentId = _departmentId, CreatedDate = DateTime.UtcNow};

        // Act
        var response =  await httpClient.PostAsJsonAsync(requestUri, deleteUserRequestEvent);

        // Assert
        var filterOptionsForAsset = new FilterOptionsForAsset();
        var serializedFilterOptionsForAsset = JsonSerializer.Serialize(filterOptionsForAsset);
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>($"/api/v1/Assets/customers/{_customerId}?filterOptions={serializedFilterOptionsForAsset}");

        Assert.NotNull(pagedAssetList);
        Assert.Null(pagedAssetList!.Items.First(a => a.Id == _assetOne).AssetHolderId);
        Assert.Equal(_departmentId, pagedAssetList!.Items.First(a => a.Id == _assetOne).ManagedByDepartmentId);
    }
}