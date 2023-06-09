using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Asset.API;
using Asset.API.ViewModels;
using Asset.IntegrationTests.Helpers;
using AssetServices.Infrastructure;
using AssetServices.Models;
using AssetServices.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using static System.StringComparison;
using AssetLifecycleType = Asset.API.ViewModels.AssetLifecycleType;
using DisposeSetting = Asset.API.ViewModels.DisposeSetting;
using Label = Asset.API.ViewModels.Label;
using LifeCycleSetting = Asset.API.ViewModels.LifeCycleSetting;
using ReturnLocation = Asset.API.ViewModels.ReturnLocation;

namespace Asset.IntegrationTests.Controllers;

public class AssetsControllerTests : IClassFixture<AssetWebApplicationFactory<Startup>>
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
    private readonly Guid _assetFour;
    private readonly Guid _assetFive;
    private readonly Guid _assetSix;
    private readonly Guid _assetSeven;
    private readonly Guid _assetEight;
    private readonly Guid _assetNine;


    private readonly AssetWebApplicationFactory<Startup> _factory;

    public AssetsControllerTests(AssetWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
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
        _assetFour = factory.ASSETLIFECYCLE_FOUR_ID;
        _assetEight = factory.ASSETLIFECYCLE_EIGHT_ID;
        _assetFive = factory.ASSETLIFECYCLE_FIVE_ID;
        _assetSix = factory.ASSETLIFECYCLE_SIX_ID;
        _assetSeven = factory.ASSETLIFECYCLE_SEVEN_ID;
        _assetNine = factory.ASSETLIFECYCLE_NINE_ID;
        _customerIdTwo = factory.COMPANY_ID_TWO;
        _factory = factory;
    }

    [Fact]
    public async Task GetAssetsForCustomerFilterByUserId()
    {
        var userId = "6d16a4cb-4733-44de-b23b-0eb9e8ae6590";
        var filterOptions = new FilterOptionsForAsset { UserId = userId };

        var json = JsonSerializer.Serialize(filterOptions);


        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(2, pagedAssetList!.Items.Count);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[0].CreatedDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[1].CreatedDate.Date);
    }

    [Fact]
    public async Task GetAssetsForCustomer()
    {
        int[] category = { 1, 2 };
        IList<Guid?> departments = new List<Guid?> { new Guid("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72") };
        IList<AssetLifecycleStatus> status =
            new List<AssetLifecycleStatus> { AssetLifecycleStatus.Active, AssetLifecycleStatus.Recycled };
        Guid[] labels = { new("D4535FA6-9EBB-4DCF-AB62-21BE01001345"), new("6031CDA2-C1CC-4593-A450-9EE6F47951D0") };


        var filterOptions = new FilterOptionsForAsset
        {
            Status = status, Label = labels, Department = departments, Category = category
        };

        var json = JsonSerializer.Serialize(filterOptions);

        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(2, pagedAssetList!.Items.Count);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[0].CreatedDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[1].CreatedDate.Date);
    }

    [Fact]
    public async Task GetAssetsForCustomer_SearchByName()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?q=bob";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(2, pagedAssetList!.Items.Count);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[0].CreatedDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[1].CreatedDate.Date);
        Assert.Equal(_factory.ASSETHOLDER_ONE_ID, pagedAssetList.Items[0].AssetHolderId);
        Assert.Equal(_factory.ASSETHOLDER_ONE_ID, pagedAssetList.Items[1].AssetHolderId);
    }
    [Fact]
    public async Task GetAssetLifecyclesForCustomerAsync_IsActiveStateIsFalse_IcludInactiveAssets()
    {
        //Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var filterOptions = new FilterOptionsForAsset
        {
            IsActiveState = false
        };

        var json = JsonSerializer.Serialize(filterOptions);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(2, pagedAssetList!.Items.Count);
        Assert.Collection(pagedAssetList.Items, 
            item => Assert.Equal("Inactive", item.AssetStatusName),
            item => Assert.Equal("Recycled", item.AssetStatusName)
            );
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomer()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(12, count);
    }
    [Fact]
    public async Task GetAllCount_WithoutCustomerIds()
    {
        // Act
        var response  = await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/count", new List<Guid>());
        var assetCounts = await response.Content.ReadFromJsonAsync<IList<CustomerAssetCount>>();

        // Assert
        Assert.Equal(2, assetCounts!.Count);
        Assert.Equal(14, assetCounts.FirstOrDefault()!.Count);

    }
    [Fact]
    public async Task GetAllCount_WithoutCustomerIdsPagination()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        // Act
        var response = await httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/count/pagination", new List<Guid>());
        var assetCounts = await response.Content.ReadFromJsonAsync<PagedModel<CustomerAssetCount>>();

        // Assert
        Assert.Equal(2, assetCounts!.TotalItems);
        Assert.Equal(14, assetCounts.Items.FirstOrDefault()!.Count);

    }
    [Fact]
    public async Task GetAllCount_ByPartnerAdmin()
    {
        // Arrange
        var customerIds = new List<Guid>()
        {
            Guid.Parse("cab4bb77-3471-4ab3-ae5e-2d4fce450f36")
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/count", customerIds);
        var assetCounts = await response.Content.ReadFromJsonAsync<IList<CustomerAssetCount>>();

        // Assert
        Assert.Equal(1, assetCounts!.Count);
        Assert.Equal(14, assetCounts!.FirstOrDefault()!.Count);

    }
    [Fact]
    public async Task GetAllCount_ByPartnerAdminPagination()
    {
        // Arrange
        var customerIds = new List<Guid>()
        {
            Guid.Parse("cab4bb77-3471-4ab3-ae5e-2d4fce450f36")
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/count/pagination", customerIds);
        var assetCounts = await response.Content.ReadFromJsonAsync<PagedModel<CustomerAssetCount>>();

        // Assert
        Assert.Equal(1, assetCounts!.TotalItems);
        Assert.Equal(14, assetCounts!.Items.FirstOrDefault()!.Count);

    }
    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithDepartment()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(9, count);
    }

    [Fact]
    public async Task ReadAssetFile_ValidateImport_TDLM()
    {
        // Arrange
        var fullFilename = Path.Combine(@"TestData", "demo_fileimport.csv");
        var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        using var content = new MultipartFormDataContent();
        content.Add(assetFileByteContent, "assetImportFile", "demo_fileimport.csv");

        var requestUri = $"/api/v1/Assets/customers/{_customerId}";
        var assetsBeforeImport = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        var transactionAssets = assetsBeforeImport!.Items.Count(x => x.LifecycleType == LifecycleType.Transactional);

        // Act
        requestUri = $"/api/v1/Assets/customers/{_customerId}/import?validateOnly=false&lifeCycleType={LifecycleType.Transactional}";
        var importResponse = await _httpClient.PostAsync(requestUri, content);
        var assetValidationResult = await importResponse.Content.ReadFromJsonAsync<AssetValidationResult>();

        requestUri = $"/api/v1/Assets/customers/{_customerId}";
        var importedAssets = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        var totaltransactionAssets = importedAssets!.Items.Count(x => x.LifecycleType == LifecycleType.Transactional);


        // Assert
        Assert.True(importResponse.IsSuccessStatusCode);
        Assert.True(totaltransactionAssets - 3  == transactionAssets);
        Assert.Equal(3, assetValidationResult!.ValidAssets.Count);
        Assert.Equal(3, assetValidationResult.InvalidAssets.Count);
        Assert.Equal("John", assetValidationResult.ValidAssets[0].ImportedUser.FirstName);
        Assert.Equal("Doe", assetValidationResult.ValidAssets[0].ImportedUser.LastName);
        Assert.Equal("john@doe.com", assetValidationResult.ValidAssets[0].ImportedUser.Email);
        Assert.Equal("+4799999999", assetValidationResult.ValidAssets[0].ImportedUser.PhoneNumber);
        Assert.Equal("2021-12-01", assetValidationResult.ValidAssets[0].PurchaseDate.ToString("yyyy-MM-dd"));
        Assert.Equal("Backoffice phone", assetValidationResult.ValidAssets[0].Label);
        Assert.Equal("Meny Roa Backoffice", assetValidationResult.ValidAssets[0].Alias);

        Assert.Equal("Missng/Incorrect Purchase Type Value. Correct Purchase Types: 'transactional personal', 'transactional non personal', 'as a service personal', 'as a service non personal'.", assetValidationResult.InvalidAssets[0].Errors[0]);
        Assert.Equal("Invalid e-mail: mail@", assetValidationResult.InvalidAssets[0].Errors[1]);
        Assert.Equal("Invalid Imei(s) 13311006051722 for mobile phone", assetValidationResult.InvalidAssets[2].Errors[0]);
        Assert.Equal("Invalid purchase date - expected format yyyy-MM-dd (2022-03-21): 05.10.2021", assetValidationResult.InvalidAssets[1].Errors[0]);
    }
    [Fact]
    public async Task ReadAssetFile_ValidateImport_Implement()
    {
        // Arrange
        var fullFilename = Path.Combine(@"TestData", "demo_fileimport.csv");
        var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        using var content = new MultipartFormDataContent();
        content.Add(assetFileByteContent, "assetImportFile", "demo_fileimport.csv");

        var requestUri = $"/api/v1/Assets/customers/{_customerId}";
        var assetsBeforeImport = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        var nolifecycleAssets = assetsBeforeImport!.Items.Count(x => x.LifecycleType == LifecycleType.NoLifecycle);

        // Act
        requestUri = $"/api/v1/Assets/customers/{_customerId}/import?validateOnly=false&lifeCycleType={LifecycleType.NoLifecycle}";
        var importResponse = await _httpClient.PostAsync(requestUri, content);
        var assetValidationResult = await importResponse.Content.ReadFromJsonAsync<AssetValidationResult>();

        requestUri = $"/api/v1/Assets/customers/{_customerId}";
        var importedAssets = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        var totalNolifeCycleAssets = importedAssets!.Items.Count(x => x.LifecycleType == LifecycleType.NoLifecycle);


        // Assert
        Assert.True(importResponse.IsSuccessStatusCode);
        Assert.True(totalNolifeCycleAssets - 3 == nolifecycleAssets);
        Assert.Equal(3, assetValidationResult!.ValidAssets.Count);
        Assert.Equal(3, assetValidationResult.InvalidAssets.Count);
        Assert.Equal("John", assetValidationResult.ValidAssets[0].ImportedUser.FirstName);
        Assert.Equal("Doe", assetValidationResult.ValidAssets[0].ImportedUser.LastName);
        Assert.Equal("john@doe.com", assetValidationResult.ValidAssets[0].ImportedUser.Email);
        Assert.Equal("+4799999999", assetValidationResult.ValidAssets[0].ImportedUser.PhoneNumber);
        Assert.Equal("2021-12-01", assetValidationResult.ValidAssets[0].PurchaseDate.ToString("yyyy-MM-dd"));
        Assert.Equal("Backoffice phone", assetValidationResult.ValidAssets[0].Label);
        Assert.Equal("Meny Roa Backoffice", assetValidationResult.ValidAssets[0].Alias);

        Assert.Equal("Missng/Incorrect Purchase Type Value. Correct Purchase Types: 'transactional personal', 'transactional non personal', 'as a service personal', 'as a service non personal'.", assetValidationResult.InvalidAssets[0].Errors[0]);
        Assert.Equal("Invalid e-mail: mail@", assetValidationResult.InvalidAssets[0].Errors[1]);
        Assert.Equal("Invalid Imei(s) 13311006051722 for mobile phone", assetValidationResult.InvalidAssets[2].Errors[0]);
        Assert.Equal("Invalid purchase date - expected format yyyy-MM-dd (2022-03-21): 05.10.2021", assetValidationResult.InvalidAssets[0].Errors[2]);
    }

    [Fact]
    public async Task ReadAssetFile_ValidateResponse()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        // Arrange
        var fullFilename = Path.Combine(@"TestData", "demo_fileimport.csv");
        var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        using var content = new MultipartFormDataContent();
        content.Add(assetFileByteContent, "assetImportFile", "demo_fileimport.csv");
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/import";

        // Act
        var importResponse = await httpClient.PostAsync(requestUri, content);
        var assetValidationResult = await importResponse.Content.ReadFromJsonAsync<AssetValidationResult>();

        // Assert
        Assert.True(importResponse.IsSuccessStatusCode);
        Assert.Equal(3, assetValidationResult!.ValidAssets.Count);
        Assert.Equal(3, assetValidationResult.InvalidAssets.Count);
        Assert.Equal("John", assetValidationResult.ValidAssets[0].ImportedUser.FirstName);
        Assert.Equal("Doe", assetValidationResult.ValidAssets[0].ImportedUser.LastName);
        Assert.Equal("john@doe.com", assetValidationResult.ValidAssets[0].ImportedUser.Email);
        Assert.Equal("+4799999999", assetValidationResult.ValidAssets[0].ImportedUser.PhoneNumber);
        Assert.Equal("2021-12-01", assetValidationResult.ValidAssets[0].PurchaseDate.ToString("yyyy-MM-dd"));
        Assert.Equal("Backoffice phone", assetValidationResult.ValidAssets[0].Label);
        Assert.Equal("Meny Roa Backoffice", assetValidationResult.ValidAssets[0].Alias);

        Assert.Equal("Missng/Incorrect Purchase Type Value. Correct Purchase Types: 'transactional personal', 'transactional non personal', 'as a service personal', 'as a service non personal'.", assetValidationResult.InvalidAssets[0].Errors[0]);
        Assert.Equal("Invalid e-mail: mail@", assetValidationResult.InvalidAssets[0].Errors[1]);
        Assert.Equal("Invalid Imei(s) 13311006051722 for mobile phone", assetValidationResult.InvalidAssets[2].Errors[0]);
        Assert.Equal("Invalid purchase date - expected format yyyy-MM-dd (2022-03-21): 05.10.2021", assetValidationResult.InvalidAssets[1].Errors[0]);
    }


    [Fact]
    public async Task ReadAssetFile_DuplicateIMEI()
    {
        // Arrange
        var fullFilename = Path.Combine(@"TestData", "demo_fileimport_duplicateIMEI.csv");
        var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        using var content = new MultipartFormDataContent();
        content.Add(assetFileByteContent, "assetImportFile", "demo_fileimport_duplicateIMEI.csv");
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/import";

        // Act
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var importResponse = await httpClient.PostAsync(requestUri, content);
        var assetValidationResult = await importResponse.Content.ReadFromJsonAsync<AssetValidationResult>();

        // Assert
        Assert.True(importResponse.IsSuccessStatusCode);
        Assert.Equal(2, assetValidationResult!.ValidAssets.Count);
        Assert.Equal(4, assetValidationResult.InvalidAssets.Count);
        Assert.Equal("John", assetValidationResult.ValidAssets[0].ImportedUser.FirstName);
        Assert.Equal("Doe", assetValidationResult.ValidAssets[0].ImportedUser.LastName);
        Assert.Equal("john@doe.com", assetValidationResult.ValidAssets[0].ImportedUser.Email);
        Assert.Equal("+4799999999", assetValidationResult.ValidAssets[0].ImportedUser.PhoneNumber);
        Assert.Equal("2021-12-01", assetValidationResult.ValidAssets[0].PurchaseDate.ToString("yyyy-MM-dd"));
        Assert.Equal("Backoffice phone", assetValidationResult.ValidAssets[0].Label);
        Assert.Equal("Meny Roa Backoffice", assetValidationResult.ValidAssets[0].Alias);

        Assert.Equal("Invalid e-mail: mail@", assetValidationResult.InvalidAssets[0].Errors[0]);
        Assert.Equal("Invalid Imei(s) 13311006051722 for mobile phone", assetValidationResult.InvalidAssets[3].Errors[1]);
        Assert.Equal("Invalid purchase date - expected format yyyy-MM-dd (2022-03-21): 05.10.2021", assetValidationResult.InvalidAssets[1].Errors[0]);  
        Assert.Equal("Duplicate Active IMEI in the System - expected UNIQUE IMEI: 500119468586675", assetValidationResult.InvalidAssets[1].Errors[1]);  
        Assert.Equal("Duplicate IMEI in the file - expected UNIQUE IMEI: 13311006051722", assetValidationResult.InvalidAssets[3].Errors[0]);
    }

    [Fact]
    public async Task ReadAssetFile_PurchasePrice()
    {
        // Arrange
        var fullFilename = Path.Combine(@"TestData", "demo_purchase_price_validation.csv");
        var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        using var content = new MultipartFormDataContent();
        content.Add(assetFileByteContent, "assetImportFile", "demo_purchase_price_validation.csv");
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/import";

        // Act
        var importResponse = await _httpClient.PostAsync(requestUri, content);
        var assetValidationResult = await importResponse.Content.ReadFromJsonAsync<AssetValidationResult>();

        // Assert
        Assert.True(importResponse.IsSuccessStatusCode);
        Assert.Equal(2, assetValidationResult!.ValidAssets.Count);
        Assert.Equal(2, assetValidationResult.InvalidAssets.Count);

        Assert.Equal("Nora", assetValidationResult.ValidAssets[0].ImportedUser.FirstName);
        Assert.Equal("Kollen", assetValidationResult.ValidAssets[0].ImportedUser.LastName);
        Assert.Equal("10000", assetValidationResult.ValidAssets[0].PurchasePrice);

        Assert.Equal("Nora", assetValidationResult.ValidAssets[1].ImportedUser.FirstName);
        Assert.Equal("Kollen", assetValidationResult.ValidAssets[1].ImportedUser.LastName);
        Assert.Equal("12220", assetValidationResult.ValidAssets[1].PurchasePrice);

        Assert.Equal("PurchasePrice is a required field and needs to be a number.", assetValidationResult.InvalidAssets[0].Errors[0]);
        Assert.Equal("Invalid amount for Purchase price: ddd -  expected number.", assetValidationResult.InvalidAssets[1].Errors[0]);
    }

    [Fact]
    public async Task ReadAssetFile_CheckSavedAsset()
    {
        //// Arrange
        //var fullFilename = Path.Combine(@"TestData", "demo_fileimport.csv");
        //var assetImportFileBytes = await File.ReadAllBytesAsync(fullFilename);
        //var assetFileByteContent = new ByteArrayContent(assetImportFileBytes);
        //using var content = new MultipartFormDataContent();
        //content.Add(assetFileByteContent, "assetImportFile", "demo_fileimport.csv");
        //var requestUri = $"/api/v1/Assets/customers/{_customerId}/import?ValidateOnly=false";

        //// Act
        //var importResponse = await _httpClient.PostAsync(requestUri, content);
        //var requestAssetUri = $"/api/v1/Assets/customers/{_customerId}?q=99999999";
        //var pagedAssetList = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestAssetUri);

        //// Assert
        //Assert.Equal(1, pagedAssetList!.Items.Count);
        //Assert.Equal(DateTime.UtcNow.Date, pagedAssetList.Items[0].CreatedDate.Date);
        //Assert.Equal("2021-12-01", pagedAssetList.Items[0].PurchaseDate.ToString("yyyy-MM-dd"));
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithStatus()
    {
        var requestUri =
            $"/api/v1/Assets/customers/{_customerId}/count?departmentId=&assetLifecycleStatus={(int)AssetLifecycleStatus.Available}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetCustomerItemCount_ForCustomerWithDepartmentAndStatus()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var requestUri =
            $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}&assetLifecycleStatus={(int)AssetLifecycleStatus.InUse}";
        _testOutputHelper.WriteLine(requestUri);
        var count = await httpClient.GetFromJsonAsync<int>(requestUri);
        Assert.Equal(3, count);
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
            Description = DESCRIPTION,
            Imei = new List<long> { FIRST_IMEI },
            CallerId = _callerId,
            PaidByCompany = new Money(PAID_BY_COMPANY, CurrencyCode.NOK),
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

        var filter = new FilterOptionsForAsset();
        var json = JsonSerializer.Serialize(filter);
        requestUri = $"/api/v1/Assets/customers/{_organizationId}?filterOptions={json}";

        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        Assert.Equal(1, pagedAssetList!.Items.Count);
        Assert.Equal(ALIAS, pagedAssetList.Items[0].Alias);
        Assert.Equal(ASSET_CATEGORY_ID, pagedAssetList.Items[0].AssetCategoryId);
        Assert.Equal(NOTE, pagedAssetList.Items[0].Note);
        Assert.Equal(BRAND, pagedAssetList.Items[0].Brand);
        Assert.Equal(PRODUCT_NAME, pagedAssetList.Items[0].ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), pagedAssetList.Items[0].LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), pagedAssetList.Items[0].PurchaseDate.ToShortDateString());
        Assert.Equal(managedByDepartmentId, pagedAssetList.Items[0].ManagedByDepartmentId);
        Assert.Equal(FIRST_IMEI, pagedAssetList.Items[0].Imei.FirstOrDefault());
        Assert.Equal(ORDER_NUMBER, pagedAssetList.Items[0].OrderNumber);
        Assert.Equal(PRODUCT_ID, pagedAssetList.Items[0].ProductId);
        Assert.Equal(INVOICE_NUMBER, pagedAssetList.Items[0].InvoiceNumber);
        Assert.Equal(TRANSACTION_ID, pagedAssetList.Items[0].TransactionId);

        requestUri = $"/api/v1/Assets/{pagedAssetList.Items[0].Id}/customers/{_organizationId}";
        var assetLifecycle = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUri);
        Assert.Equal(ALIAS, assetLifecycle!.Alias);
        Assert.Equal(ASSET_CATEGORY_ID, assetLifecycle.AssetCategoryId);
        Assert.Equal("Mobile phone", assetLifecycle.AssetCategoryName);
        Assert.Equal(NOTE, assetLifecycle.Note);
        Assert.Equal(BRAND, assetLifecycle.Brand);
        Assert.Equal(PRODUCT_NAME, assetLifecycle.ProductName);
        Assert.Equal(LifecycleType.Transactional.ToString(), assetLifecycle.LifecycleName);
        Assert.Equal(purchaseDate.ToShortDateString(), assetLifecycle.PurchaseDate.ToShortDateString());
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
    public async Task CreateAssetWithoutPurchasedBy()
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
    public async Task CreateAssetWithPurchasedBy()
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
            CallerId = _callerId,
            PurchasedBy = "techstep"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(assetReturned!.PurchasedBy, newAsset.PurchasedBy);
    }

    [Fact]
    public async Task CreateAssetWithIsPersonalSetAndNoAssetTag()
    {
        var newAsset = new NewAsset
        {
            AssetCategoryId = 1,
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2022, 2, 2),
            Imei = new List<long> { 356728115537645 },
            IsPersonal = true,
            CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.True(assetReturned!.IsPersonal);
    }

    [Fact]
    public async Task CreateAssetWithFileImportedSource()
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
            CallerId = _callerId,
            Source = "FileImport"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal("FileImport", assetReturned!.Source);
    }

    [Fact]
    public async Task CreateAssetWithIncorrectSource()
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
            CallerId = _callerId,
            Source = "NOT_EXISTS"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        _testOutputHelper.WriteLine(await createResponse.Content.ReadAsStringAsync());
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal("Unknown", assetReturned!.Source);
    }

    [Fact]
    public async Task CreateNoLifecycleAssetWithoutOwner_ShouldBeNonPersonal()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetCategoryId = 1,
            Note = "A long note",
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.NoLifecycle,
            PurchaseDate = new DateTime(2022, 2, 2),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId,
            Source = "WEBSHOP"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        _testOutputHelper.WriteLine(await createResponse.Content.ReadAsStringAsync());
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.False(assetReturned!.IsPersonal);
    }

    [Fact]
    public async Task CreateNoLifecycleAssetWithOwner_ShouldGetActiveStatus()
    {
        var newAsset = new NewAsset
        {
            
            AssetCategoryId = 1,
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.NoLifecycle,
            PurchaseDate = new DateTime(2022, 2, 2),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId,
            Source = "WEBSHOP"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        _testOutputHelper.WriteLine(await createResponse.Content.ReadAsStringAsync());
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.False(assetReturned!.IsPersonal);
        Assert.NotNull(assetReturned);
        Assert.Equal(AssetLifecycleStatus.Active,assetReturned!.AssetStatus);
        Assert.Equal(LifecycleType.NoLifecycle, assetReturned!.LifecycleType);
        Assert.Equal(default(DateTime), assetReturned!.StartPeriod);
        Assert.Equal(default(DateTime), assetReturned!.EndPeriod);
    }

    [Fact]
    public async Task CreateBYODAssetWithoutOwner_ShouldBeNonPersonal()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetCategoryId = 1,
            Note = "A long note",
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.BYOD,
            PurchaseDate = new DateTime(2022, 2, 2),
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId,
            Source = "WEBSHOP"
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newAsset));
        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        _testOutputHelper.WriteLine(await createResponse.Content.ReadAsStringAsync());
        var assetReturned = await createResponse.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.False(assetReturned!.IsPersonal);
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
        var userId = Guid.Parse("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        var customerId = Guid.Parse("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");

        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var asset = new AssignAssetToUser { UserId = userId, CallerId = _callerId };
        var requestUriUser = $"/api/v1/Assets/{_assetTwo}/customer/{customerId}/assign";
        var response = await httpClient.PostAsync(requestUriUser, JsonContent.Create(asset));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = new UnAssignAssetToUser { CallerId = _callerId, DepartmentId = _departmentId };


        var requestUri = $"/api/v1/Assets/customers/{customerId}/users/{userId}";
        var deleteResponse = await httpClient.PatchAsync(requestUri, JsonContent.Create(data));
        Assert.Equal(HttpStatusCode.Accepted, deleteResponse.StatusCode);

        var asset1 = new FilterOptionsForAsset();
        var json = JsonSerializer.Serialize(asset1);
        var pagedAssetList =
            await httpClient.GetFromJsonAsync<PagedAssetList>(
                $"/api/v1/Assets/customers/{customerId}?filterOptions={json}");

        Assert.NotNull(pagedAssetList);
        Assert.Equal(14, pagedAssetList!.TotalItems);
        Assert.Collection(pagedAssetList.Items, item => Assert.Null(item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Null(item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId),
            item => Assert.Equal(data.DepartmentId, item.ManagedByDepartmentId));
        Assert.All(pagedAssetList.Items, m => Assert.Null(m.AssetHolderId));
    }

    [Fact]
    public async Task GetLifeCycleSetting()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var setting = await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(requestUri);
        Assert.Equal(1, setting!.Count);
    }

    [Fact]
    public async Task LifeCycleSetting_ShouldReturnEmpty_IfNotExists()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerIdTwo}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var setting = await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(requestUri);
        Assert.Equal(0, setting!.Count);
    }

    [Fact]
    public async Task CreateLifeCycleSetting()
    {
        var newSettings = new NewLifeCycleSetting
        {
            AssetCategoryId = 1, BuyoutAllowed = true, Runtime = 12, CallerId = _callerId
        };
        var customerId = Guid.NewGuid();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);

        var setting =
            await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(
                $"/api/v1/Assets/customers/{customerId}/lifecycle-setting");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.BuyoutAllowed ==
                    newSettings.BuyoutAllowed);
        Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.Runtime ==
                    newSettings.Runtime);
    }

    [Fact]
    public async Task CreateLifeCycleSetting_DefaultRuntime()
    {
        var newSettings = new NewLifeCycleSetting
        {
            AssetCategoryId = 1,
            BuyoutAllowed = true,
            CallerId = _callerId
        };
        var customerId = Guid.NewGuid();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);

        var setting =
            await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(
                $"/api/v1/Assets/customers/{customerId}/lifecycle-setting");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.BuyoutAllowed ==
                    newSettings.BuyoutAllowed);
        Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.Runtime == 36);
    }
    [Fact]
    public async Task GetAvailableAssetsForCustomer()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var options = new FilterOptionsForAsset();
        var json = JsonSerializer.Serialize(options);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?page=1&limit=1000&status=9&filterOptions={json}";
        _testOutputHelper.WriteLine(requestUri);
        var pagedAsset = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        Assert.True(pagedAsset!.TotalItems == 14);
        Assert.True(pagedAsset.Items.Count == 14);
        Assert.True(pagedAsset.Items.Count(x => x.AssetStatus == AssetLifecycleStatus.InUse) == 4);
    }
    
    [Fact]
    public async Task Get_AssetsInActiveState_CompairAssetCountEndpointAndGetAssetEndpoint()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var options = new FilterOptionsForAsset
        {
            IsActiveState = true
        };
        var json = JsonSerializer.Serialize(options);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?page=1&limit=25&filterOptions={json}";
        _testOutputHelper.WriteLine(requestUri);
        var pagedAsset = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        Assert.True(pagedAsset!.TotalItems == 12);
        Assert.True(pagedAsset.Items.Count == 12);

        var requestGet = $"/api/v1/Assets/customers/{_customerId}/assets-counter";
        var responseGet = await httpClient.GetAsync(requestGet);

        var assetsCounter = await responseGet.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(2, assetsCounter?.NonPersonal?.InUse);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Active);
        Assert.Equal(1, assetsCounter?.NonPersonal?.InputRequired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Available);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Expired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Repair);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingReturn);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingRecycle);
        Assert.Equal(1, assetsCounter?.NonPersonal?.ExpiresSoon);

        Assert.Equal(2, assetsCounter?.Personal?.InUse);
        Assert.Equal(0, assetsCounter?.Personal?.Active);
        Assert.Equal(0, assetsCounter?.Personal?.InputRequired);
        Assert.Equal(0, assetsCounter?.Personal?.Available);
        Assert.Equal(0, assetsCounter?.Personal?.Expired);
        Assert.Equal(0, assetsCounter?.Personal?.Repair);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
        Assert.Equal(0, assetsCounter?.Personal?.ExpiresSoon);

        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);
        Assert.Empty(assetsCounter?.Departments);
    }
    [Fact]
    public async Task MakeAssetAvailableAsync()
    {
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetThree, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-available";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Available);
        Assert.True(updatedAsset.AssetHolderId == null || updatedAsset.AssetHolderId == Guid.Empty);
        Assert.True(updatedAsset.Labels == null || !updatedAsset.Labels.Any());
    }
    [Fact]
    public async Task MakeAssetAvailableAsync_PreviousOwner_ShouldSendMail()
    {
        var postData = new MakeAssetAvailable
        {
            AssetLifeCycleId = _assetFour,
            CallerId = _callerId,
            PreviousUser = new EmailPersonAttribute
            {
                Email = "test@test.com",
                Name = "Test",
                PreferedLanguage = "no"
            }
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-available";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
    }
    [Fact]
    public async Task MakeAssetAvailableAsync_PreviousOwnerEmptyStrings_ShouldNotSendMail()
    {
        var postData = new MakeAssetAvailable
        {
            AssetLifeCycleId = _assetFour,
            CallerId = _callerId,
            PreviousUser = new EmailPersonAttribute
            {
                Email = "",
                Name = "",
                PreferedLanguage = "no"
            }
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-available";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
    }
    [Fact]
    public async Task MakeAssetAvailableAsync_PreviousOwner_PreferencesDefault()
    {
        var postData = new MakeAssetAvailable
        {
            AssetLifeCycleId = _assetFour,
            CallerId = _callerId,
            PreviousUser = new EmailPersonAttribute
            {
                Email = "mail@test.com",
                Name = "Test"
            }
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-available";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
    }
    [Fact]
    public async Task MakeAssetAvailableAsync_PreviousOwner_NoMailAndNameValue() 
    {
        var postData = new MakeAssetAvailable
        {
            AssetLifeCycleId = _assetFour,
            CallerId = _callerId,
            PreviousUser = new EmailPersonAttribute
            {
                PreferedLanguage = "no"
            }
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-available";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
    }

    [Fact]
    public async Task MakeAssetExpireAsync_AssetNotFound()
    {
        var postData = new MakeAssetAvailable { AssetLifeCycleId = Guid.NewGuid(), CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task MakeAssetExpireAsync_AssetNotActive()
    {
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetFour, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task MakeAssetExpireAsync_NoEndPeriod()
    {
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetFive, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task MakeAssetExpireAsync_NotExpiring()
    {
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetSix, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }
    [Fact]
    public async Task MakeAssetExpiresSoon_AlreadyExpired()
    {
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetNine, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetNine, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var responsePost = await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var errorResponse = await responsePost.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
        Assert.Equal($"Asset already 'Expired'", errorResponse?.Message);

    }
    [Fact]
    public async Task MakeAssetExpireAsync()
    {
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetNine, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var postData = new MakeAssetAvailable { AssetLifeCycleId = _assetNine, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/make-expire";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Expired);
        Assert.True(updatedAsset!.IsActiveState);
    }

    [Fact]
    public async Task ReturnDeviceAsync_PersonalPendingReturn()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        await httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var postData = new ReturnDevice { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.PendingReturn);
    }

    [Fact]
    public async Task ReturnDeviceAsync_NonPersonalConfirmReturn()
    {
        var disposeSetting =
            await _httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerId}/dispose-setting");
        var postData = new ReturnDevice
        {
            AssetLifeCycleId = _assetThree,
            CallerId = _callerId,
            ReturnLocationId = disposeSetting!.ReturnLocations.FirstOrDefault()!.Id
        };
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetThree, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Returned);
    }

    [Fact]
    public async Task ReturnDeviceAsync_AssetNotFound()
    {
        var postData = new ReturnDevice { AssetLifeCycleId = _assetEight, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task ReturnDeviceAsync_PersonalConfirmReturn()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        await httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var disposeSetting =
            await httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerId}/dispose-setting");
        var postData = new ReturnDevice
        {
            AssetLifeCycleId = _assetOne,
            CallerId = _callerId,
            ReturnLocationId = disposeSetting!.ReturnLocations.FirstOrDefault()!.Id
        };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var confirmPost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await confirmPost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.Equal(HttpStatusCode.OK, confirmPost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Returned);
    }

    [Fact]
    public async Task ReturnDeviceAsync_InactiveAsset()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var disposeSetting =
            await httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerId}/dispose-setting");
        var postData = new ReturnDevice
        {
            AssetLifeCycleId = _assetOne,
            CallerId = _callerId,
            ReturnLocationId = disposeSetting!.ReturnLocations.FirstOrDefault()!.Id,
            Role = PredefinedRole.Manager.ToString()
        };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var confirmPost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, confirmPost.StatusCode);
    }
    [Fact]
    public async Task ReturnDeviceAsync_DirectReturnByManager()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var disposeSetting =
            await httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerId}/dispose-setting");
        var postData = new ReturnDevice
        {
            AssetLifeCycleId = _assetOne,
            CallerId = _callerId,
            ReturnLocationId = disposeSetting!.ReturnLocations.FirstOrDefault()!.Id,
            Role = PredefinedRole.Manager.ToString()
        };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Returned);
    }
    [Fact]
    public async Task ReturnDeviceAsync_NoReturnLocation()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var postData = new ReturnDevice
        {
            AssetLifeCycleId = _assetOne,
            CallerId = _callerId,
            ReturnLocationId = Guid.NewGuid(),
            Role = PredefinedRole.Manager.ToString()
        };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }
    [Fact]
    public async Task PendigBuyoutDeviceAsync()
    {
        var postData = new PendingBuyoutDeviceDTO { AssetLifeCycleId = _ = _assetOne, LasWorkingDay = DateTime.UtcNow.AddMonths(-6), User = null, Currency = CurrencyCode.NOK.ToString(), CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/pending-buyout";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.PendingBuyout);
        Assert.True(updatedAsset!.OffboardBuyoutPrice.Amount == 1510.41m);
    }
    [Fact]
    public async Task PendigBuyoutDeviceAsync_ManagerPerforming()
    {
        var postData = new PendingBuyoutDeviceDTO 
        { 
            AssetLifeCycleId = _ = _assetOne, 
            LasWorkingDay = DateTime.UtcNow.AddMonths(-6), 
            User = new EmailPersonAttributeDTO()
            {
                Email = "test@techstep.no",
                Name = "System",
                PreferedLanguage = "NO"
            }, 
            CallerId = _callerId,
            Currency = CurrencyCode.NOK.ToString(),
            Role = PredefinedRole.Manager.ToString()
        };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/pending-buyout";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.PendingBuyout);
        Assert.True(updatedAsset!.OffboardBuyoutPrice.Amount == 1510.41m);
    }
    [Fact]
    public async Task ConfirmBuyoutDeviceAsync()
    {
        // Arrange
        var postData = new PendingBuyoutDeviceDTO { AssetLifeCycleId = _assetOne, LasWorkingDay = DateTime.UtcNow.AddMonths(-6), Currency = CurrencyCode.NOK.ToString(),  User = null, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/pending-buyout";
        _testOutputHelper.WriteLine(requestUri);
        await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var buyoutData = new BuyoutDeviceDTO { AssetLifeCycleId = _assetOne, CallerId = _callerId };

        // Act
        requestUri = $"/api/v1/Assets/customers/{_customerId}/confirm-buyout";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(buyoutData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);   
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.BoughtByUser);
    }

    [Fact]
    public async Task BuyoutDeviceAsync()
    {
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var postData = new BuyoutDeviceDTO { AssetLifeCycleId = _ = _assetOne, CallerId = _callerId, PayrollContactEmail = "test@techstep.no" };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/buyout-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.BoughtByUser);
    }

    [Fact]
    public async Task BuyoutDeviceAsync_IsNonPersonal()
    {
        var postData = new BuyoutDeviceDTO { AssetLifeCycleId = _assetFive, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/buyout-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task BuyoutDeviceAsync_IsInactive()
    {
        var postData = new BuyoutDeviceDTO { AssetLifeCycleId = _assetSeven, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/buyout-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task BuyoutDeviceAsync_PayrollEmailNotExist()
    {
        var postData = new BuyoutDeviceDTO { AssetLifeCycleId = _assetEight, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerIdTwo}/buyout-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task ReportDeviceAsync_IsInactive()
    {
        var postData = new BuyoutDeviceDTO { AssetLifeCycleId = _assetSeven, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/customers/{_customerIdTwo}/report-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task ReportDeviceAsync_Lost()
    {
        var postData = new ReportDevice
        {
            AssetLifeCycleId = _assetThree,
            ReportCategory = ReportCategory.Lost,
            Description = "description",
            TimePeriodFrom = Convert.ToDateTime("02-02-20"),
            TimePeriodTo = Convert.ToDateTime("04-02-20"),
            Country = "NO",
            Address = "Address",
            PostalCode = "0456",
            City = "Oslo",
            CallerId = _callerId
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/report-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Lost);
    }

    [Fact]
    public async Task ReportDeviceAsync_Stolen()
    {
        var postData = new ReportDevice
        {
            AssetLifeCycleId = _assetThree,
            ReportCategory = ReportCategory.Stolen,
            Description = "description",
            TimePeriodFrom = Convert.ToDateTime("02-02-20"),
            TimePeriodTo = Convert.ToDateTime("04-02-20"),
            Country = "NO",
            Address = "Address",
            PostalCode = "0456",
            City = "Oslo",
            CallerId = _callerId
        };

        var requestUri = $"/api/v1/Assets/customers/{_customerId}/report-device";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await _httpClient.PostAsync(requestUri, JsonContent.Create(postData));
        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.True(updatedAsset!.AssetStatus == AssetLifecycleStatus.Stolen);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "40ca1747-5dd3-41f1-9301-d7eafa4ee09b")]
    [InlineData("40ca1747-5dd3-41f1-9301-d7eafa4ee09b", "00000000-0000-0000-0000-000000000000")]
    [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    [InlineData("40ca1747-5dd3-41f1-9301-d7eafa4ee09b", "40ca1747-5dd3-41f1-9301-d7eafa4ee09b")]
    public async Task ReAssignAssetToHolder_PersonalWithError(string userId, string deptId)
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var postData = new ReAssignAsset
        {
            Personal = true, UserId = Guid.Parse(userId), DepartmentId = Guid.Parse(deptId), CallerId = _callerId
        };

        // Act
        var requestUri = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/re-assignment";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        // Assert
        if (postData.UserId != Guid.Empty && postData.DepartmentId != Guid.Empty)
        {
            Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
        }
    }

    [Fact]
    public async Task ReAssignAssetToHolder_WithUserAndDepartment_ShouldSetToPersonal()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var reAssignAsset = new ReAssignAsset { UserId = _user, DepartmentId = _departmentIdTwo, CallerId = _callerId };

        // Act
        var requestUri = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/re-assignment";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(reAssignAsset));

        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.Equal(AssetLifecycleStatus.InUse, updatedAsset!.AssetStatus);
        Assert.Equal(reAssignAsset.UserId, updatedAsset.AssetHolderId);
        Assert.Null(updatedAsset.ManagedByDepartmentId);
        Assert.True(updatedAsset.IsPersonal);
    }

    [Theory]
    [InlineData("40ca1747-5dd3-41f1-9301-d7eafa4ee09b", "00000000-0000-0000-0000-000000000000")]
    [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    public async Task ReAssignAssetToHolder_NonPersonalWithError(string userId, string deptId)
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var postData = new ReAssignAsset
        {
            Personal = false, UserId = Guid.Parse(userId), DepartmentId = Guid.Parse(deptId), CallerId = _callerId
        };

        // Act
        var requestUri = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/re-assignment";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(postData));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responsePost.StatusCode);
    }

    [Fact]
    public async Task ReAssignAssetToHolder_NonPersonal()
    {
        // Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var reAssignAsset = new ReAssignAsset
        {
            Personal = false, DepartmentId = _departmentIdTwo, CallerId = _callerId
        };

        // Act
        var requestUri = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/re-assignment";
        _testOutputHelper.WriteLine(requestUri);
        var responsePost = await httpClient.PostAsync(requestUri, JsonContent.Create(reAssignAsset));

        var updatedAsset = await responsePost.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        Assert.Equal(AssetLifecycleStatus.InUse, updatedAsset!.AssetStatus);
        Assert.Null(updatedAsset.AssetHolderId);
        Assert.Equal(reAssignAsset.DepartmentId, updatedAsset.ManagedByDepartmentId);
        Assert.Equal(reAssignAsset.Personal, updatedAsset.IsPersonal);
    }


    [Fact]
    public async Task UpdateLifeCycleSetting()
    {
        var newSettings = new NewLifeCycleSetting { AssetCategoryId = 1, BuyoutAllowed = false, CallerId = _callerId };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
        _testOutputHelper.WriteLine(requestUri);
        await _httpClient.PutAsJsonAsync(requestUri, newSettings);
        var setting =
            await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(
                $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");

        Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.BuyoutAllowed ==
                    newSettings.BuyoutAllowed);
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("NO", null)]
    [InlineData("NO", 1)]
    [InlineData(null, 1)]
    public async Task GetBaseMinBuyoutPrice(string? country, int? assetCategoryId)
    {
        // Arrange
        _factory.WithWebHostBuilder(builder =>
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
            Assert.True(buyoutPrices!.All(x => string.Equals(x.Country, country, CurrentCultureIgnoreCase)));
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
        Assert.Equal(4, labelsList!.Count);
        Assert.Contains(labelsList, l => string.Equals(l.Text, "Label1", InvariantCulture));

        var labelGuid = new List<Guid> { labelsList[0].Id, labelsList[1].Id };
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
        var filterOptionsForAsset = new FilterOptionsForAsset();
        var filterOptions = JsonSerializer.Serialize(filterOptionsForAsset);
        var requestAllAssets = $"/api/v1/Assets/customers/{_customerId}?filterOptions={filterOptions}";
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestAllAssets);
        Assert.Equal(14, pagedAssetList!.Items.Count);
        var assetOneFromResponse = pagedAssetList.Items.FirstOrDefault(i => i.Id == _assetOne);
        Assert.Equal(_assetOne, assetOneFromResponse!.Id);
        Assert.Equal(2, assetOneFromResponse.Labels.Count);

        // Delete label for customer
        var requestUriLabelDelete = $"{httpClient.BaseAddress}api/v1/Assets/customers/{_customerId}/labels";
        var labelsToDelete = new List<Guid> { labelsList[0].Id };
        var dataToDeleteRequestBody = new DeleteCustomerLabelsData { CallerId = Guid.NewGuid(), LabelGuids = labelsToDelete };
        var deleteRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestUriLabelDelete),
            Content = new StringContent(JsonSerializer.Serialize(dataToDeleteRequestBody), Encoding.UTF8, "application/json")
        };
        var responseDelete = await httpClient.SendAsync(deleteRequestMessage);
        Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);

        // Re-fetch all assets
        pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestAllAssets);
        assetOneFromResponse = pagedAssetList!.Items.FirstOrDefault(i => i.Id == _assetOne);
        Assert.Equal(1, assetOneFromResponse!.Labels.Count);
    }

    [Fact]
    public async Task GetLabels_CheckViewModel()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/labels";

        var labels = await _httpClient.GetFromJsonAsync<List<Label>>(requestUri);
        Assert.NotNull(labels);
        Assert.Equal("CompanyOne", labels?[0].Text);
        Assert.Equal("Lightblue", labels?[0].ColorName);
        Assert.NotEqual(Guid.Empty, labels?[0].Id);
    }

    [Fact]
    public async Task CreateAsset_NoAssignmentToUserOrDepartment()
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
            ManagedByDepartmentId = Guid.Empty,
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId
        };


        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        var response = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(AssetLifecycleStatus.InputRequired, asset?.AssetStatus);
    }

    [Fact]
    public async Task CreateAsset_AssignToDepartment()
    {
        var newAsset = new NewAsset
        {
            Alias = "Just another name",
            AssetTag = "A4010",
            AssetCategoryId = 1,
            Description = "A long description",
            Brand = "iPhone",
            ProductName = "12 Pro Max",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2022, 2, 2),
            ManagedByDepartmentId = _departmentId,
            Imei = new List<long> { 356728115537645 },
            CallerId = _callerId
        };


        var requestUri = $"/api/v1/Assets/customers/{_organizationId}";
        var response = await _httpClient.PostAsJsonAsync(requestUri, newAsset);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(AssetLifecycleStatus.InUse, asset?.AssetStatus);


        var requestUriAsset = $"/api/v1/Assets/{asset?.Id}/customers/{_organizationId}";
        var departmentAsset = await _httpClient.GetFromJsonAsync<API.ViewModels.Asset>(requestUriAsset);
        Assert.Equal(_departmentId, departmentAsset?.ManagedByDepartmentId);
    }

    [Fact]
    public async Task PatchAsset_AssignToDepartment()
    {
        var assignment = new AssignAssetToUser { DepartmentId = _departmentId, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(_departmentId, asset?.ManagedByDepartmentId);
        Assert.Null(asset?.AssetHolderId);
        Assert.False(asset?.IsPersonal);
    }

    [Fact]
    public async Task PatchAsset_AssignToUser()
    {
        var assignment = new AssignAssetToUser { UserId = _user, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(_user, asset?.AssetHolderId);
        Assert.Null(asset?.ManagedByDepartmentId);
        Assert.True(asset?.IsPersonal);
    }

    [Fact]
    public async Task PatchAsset_AssignToNoOne()
    {
        var assignment = new AssignAssetToUser { CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PatchAsset_UpdateBrand()
    {
        // Arrange
        const string NEW_BRAND_NAME = "New brand name";

        var updateAsset = new UpdateAsset { Brand = NEW_BRAND_NAME, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}/Update";

        // Act
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateAsset);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(NEW_BRAND_NAME, asset!.Brand);
    }

    [Fact]
    public async Task PatchAsset_UpdateAlias()
    {
        // Arrange
        const string NEW_ALIAS_NAME = "New alias";

        var updateAsset = new UpdateAsset { Alias = NEW_ALIAS_NAME, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}/Update";

        // Act
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateAsset);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(NEW_ALIAS_NAME, asset!.Alias);
    }

    [Fact]
    public async Task PatchAsset_UpdatePurchaseDate()
    {
        // Arrange
        var newPurchaseDate = new DateTime(2021, 12, 12);

        var updateAsset = new UpdateAsset { PurchaseDate = newPurchaseDate, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}/Update";

        // Act
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateAsset);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(newPurchaseDate, asset!.PurchaseDate);
    }

    [Fact]
    public async Task PatchAsset_UpdateSerialNumber()
    {
        // Arrange
        const string NEW_SERIAL_NUMBER = "123456666444";

        var updateAsset = new UpdateAsset { SerialNumber = NEW_SERIAL_NUMBER, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}/Update";

        // Act
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateAsset);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(NEW_SERIAL_NUMBER, asset!.SerialNumber);
    }

    [Fact]
    public async Task PatchAsset_UpdateNote()
    {
        // Arrange
        const string NEW_NOTE = "New note";

        var updateAsset = new UpdateAsset { Note = NEW_NOTE, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}/Update";

        // Act
        var response = await _httpClient.PostAsJsonAsync(requestUri, updateAsset);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(NEW_NOTE, asset!.Note);
    }

    [Fact]
    public async Task PatchAsset_AssignToMultiple()
    {
        var assignment = new AssignAssetToUser { CallerId = _callerId, DepartmentId = _customerId, UserId = _user };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AssignAssetLifeCycleToHolder_ToAUser()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var asset = new AssignAssetToUser { UserId = _user, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetTwo}/customer/{_customerId}/assign";
        var response = await httpClient.PostAsync(requestUri, JsonContent.Create(asset));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assignedAsset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(AssetLifecycleStatus.InUse, assignedAsset?.AssetStatus);
    }

    [Fact]
    public async Task SendToRepair()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetTwo}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetTwo, assetLifeCycle?.Id);
        Assert.Equal(AssetLifecycleStatus.Available, assetLifeCycle?.AssetStatus);

        var requestUri = $"/api/v1/Assets/{_assetTwo}/send-to-repair";
        var response = await httpClient.PatchAsync(requestUri, JsonContent.Create(_callerId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var requestGetAssetLifeCycleAfter = $"/api/v1/Assets/{_assetTwo}/customers/{_customerId}";
        var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycleAfter);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycleAfter.StatusCode);
        var assetLifeCycleAfter =
            await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycleAfter?.AssetStatus);
    }

    [Fact]
    public async Task SendToRepair_NotValidGuidForAsset_ReturnsNotFound()
    {
        //Arrange
        var NOT_VALID_GUID = Guid.NewGuid();
        var requestUri = $"/api/v1/Assets/{NOT_VALID_GUID}/send-to-repair";

        //Act
        var response = await _httpClient.PatchAsync(requestUri, JsonContent.Create(_callerId));
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            $"Asset lifecycle with id {NOT_VALID_GUID} not found",
            errorResponse?.Message);
    }

    [Fact]
    public async Task SendToRepair_NotValidStatusForSendingAssetOnRepair_ExceptionHandeling()
    {
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var body = new BuyoutDeviceDTO { AssetLifeCycleId = _assetOne, CallerId = _callerId, PayrollContactEmail ="test@techstep.no" };
        var buyoutRequest = $"/api/v1/Assets/customers/{_customerId}/buyout-device";
        var buyoutResponse = await _httpClient.PostAsJsonAsync(buyoutRequest, body);

        var requestUri = $"/api/v1/Assets/{_assetOne}/send-to-repair";
        var response = await _httpClient.PatchAsync(requestUri, JsonContent.Create(_callerId));
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid asset lifecycle status: BoughtByUser for sending asset lifecycle on repair.",
            errorResponse?.Message);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_NewAssetShouldBeMade()
    {
        //Retrive AssetLifeCycle to check if that a new asset w guid is made
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetEight}/customers/{_customerIdTwo}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetEight, assetLifeCycle?.Id);
        Assert.NotNull(assetLifeCycle?.Imei);
        Assert.NotNull(assetLifeCycle?.SerialNumber);
        Assert.NotNull(assetLifeCycle?.MacAddress);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);


        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = false,
            CallerId = _callerId,
            NewSerialNumber = "12345678975212",
            NewImei = new List<string> { "516768095487517" }
        };
        var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
        var response = await _httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //After
        var responseGetAssetLifeCycleAfter = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        var assetLifeCycleAfter =
            await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycleAfter);
        Assert.Equal(_assetEight, assetLifeCycleAfter?.Id);
        Assert.NotEqual(assetLifeCycle?.Imei, assetLifeCycleAfter?.Imei);
        Assert.NotEqual(assetLifeCycle?.SerialNumber, assetLifeCycleAfter?.SerialNumber);
        Assert.Equal(assetLifeCycle?.MacAddress, assetLifeCycleAfter?.MacAddress);
        Assert.Equal(assetLifeCycle?.Brand, assetLifeCycleAfter?.Brand);
        Assert.Equal(assetLifeCycle?.ProductName, assetLifeCycleAfter?.ProductName);
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifeCycleAfter?.AssetStatus);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_NewAssetShouldBeMade_OnlyWithNewSerialNumber()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Retrive AssetLifeCycle to check if that a new asset w guid is made
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetEight}/customers/{_customerIdTwo}";
        var responseGetAssetLifeCycle = await httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetEight, assetLifeCycle?.Id);

        Assert.NotNull(assetLifeCycle?.Imei);
        Assert.NotNull(assetLifeCycle?.SerialNumber);
        Assert.NotNull(assetLifeCycle?.MacAddress);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);


        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = false, CallerId = _callerId, NewSerialNumber = "12345678975212"
        };
        var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
        var response = await httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //After
        var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycle);
        var assetLifeCycleAfter =
            await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycleAfter);
        Assert.Equal(_assetEight, assetLifeCycleAfter?.Id);
        Assert.Equal(assetLifeCycle?.Imei, assetLifeCycleAfter?.Imei);
        Assert.NotEqual(assetLifeCycle?.SerialNumber, assetLifeCycleAfter?.SerialNumber);
        Assert.Equal(assetLifeCycle?.MacAddress, assetLifeCycleAfter?.MacAddress);
        Assert.Equal(assetLifeCycle?.Brand, assetLifeCycleAfter?.Brand);
        Assert.Equal(assetLifeCycle?.ProductName, assetLifeCycleAfter?.ProductName);
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifeCycleAfter?.AssetStatus);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_NewAssetShouldBeMade_OnlyWithImei()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Retrive AssetLifeCycle to check if that a new asset w guid is made
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetEight}/customers/{_customerIdTwo}";
        var responseGetAssetLifeCycle = await httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetEight, assetLifeCycle?.Id);

        Assert.NotNull(assetLifeCycle?.Imei);
        Assert.NotNull(assetLifeCycle?.SerialNumber);
        Assert.NotNull(assetLifeCycle?.MacAddress);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);


        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = false,
            CallerId = _callerId,
            NewImei = new List<string> { "516768095487517", "524715417699766" }
        };
        var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
        var response = await httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //After
        var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycle);
        var assetLifeCycleAfter =
            await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycleAfter);
        Assert.Equal(_assetEight, assetLifeCycleAfter?.Id);
        Assert.NotEqual(assetLifeCycle?.Imei, assetLifeCycleAfter?.Imei);
        Assert.Equal(2, assetLifeCycleAfter?.Imei.Count());
        Assert.Equal(assetLifeCycle?.SerialNumber, assetLifeCycleAfter?.SerialNumber);
        Assert.Equal(assetLifeCycle?.MacAddress, assetLifeCycleAfter?.MacAddress);
        Assert.Equal(assetLifeCycle?.Brand, assetLifeCycleAfter?.Brand);
        Assert.Equal(assetLifeCycle?.ProductName, assetLifeCycleAfter?.ProductName);
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifeCycleAfter?.AssetStatus);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_Discarded_NoNewAssetMade()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Retrive AssetLifeCycle to check if that a new asset w guid is made
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetEight}/customers/{_customerIdTwo}";
        var responseGetAssetLifeCycle = await httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetEight, assetLifeCycle?.Id);
        Assert.NotNull(assetLifeCycle?.Imei);
        Assert.NotNull(assetLifeCycle?.SerialNumber);
        Assert.NotNull(assetLifeCycle?.MacAddress);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);


        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = true, CallerId = _callerId
        };
        var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
        var response = await httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //After
        var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycle);
        var assetLifeCycleAfter =
            await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycleAfter);
        Assert.Equal(_assetEight, assetLifeCycleAfter?.Id);
        Assert.Equal(assetLifeCycle?.Imei, assetLifeCycleAfter?.Imei);
        Assert.Equal(assetLifeCycle?.SerialNumber, assetLifeCycleAfter?.SerialNumber);
        Assert.Equal(assetLifeCycle?.MacAddress, assetLifeCycleAfter?.MacAddress);
        Assert.Equal(assetLifeCycle?.Brand, assetLifeCycleAfter?.Brand);
        Assert.Equal(assetLifeCycle?.ProductName, assetLifeCycleAfter?.ProductName);
        Assert.Equal(AssetLifecycleStatus.Discarded, assetLifeCycleAfter?.AssetStatus);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_NotValidStatusForCompletingTheReturn_BadRequestWithMessage()
    {
        var expiresData = new MakeAssetExpired { AssetLifeCycleId = _assetOne, CallerId = _callerId };
        await _httpClient.PostAsJsonAsync($"/api/v1/Assets/customers/{_customerId}/make-expiressoon", expiresData);
        var body = new BuyoutDeviceDTO { AssetLifeCycleId = _assetOne, CallerId = _callerId, PayrollContactEmail = "test@techstep.no" };
        var buyoutRequest = $"/api/v1/Assets/customers/{_customerId}/buyout-device";
        var buyoutResponse = await _httpClient.PostAsJsonAsync(buyoutRequest, body);

        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = false, CallerId = _callerId
        };
        var requestUri = $"/api/v1/Assets/{_assetOne}/repair-completed";
        var response = await _httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid asset lifecycle status: BoughtByUser for completing return.",
            errorResponse?.Message);
    }

    [Fact]
    public async Task AssetLifeCycleRepairCompleted_NewAssetShouldBeMade_InvalidImeiBadRequest()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetEight}/customers/{_customerIdTwo}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetEight, assetLifeCycle?.Id);

        Assert.NotNull(assetLifeCycle?.Imei);
        Assert.NotNull(assetLifeCycle?.SerialNumber);
        Assert.NotNull(assetLifeCycle?.MacAddress);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);

        var INVALID_IMEI = "022002044";
        var assetLifeCycleRepairCompleted = new AssetLifeCycleRepairCompleted
        {
            Discarded = false, CallerId = _callerId, NewImei = new List<string> { INVALID_IMEI }
        };
        var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
        var response = await _httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"Invalid imei: {INVALID_IMEI}", errorResponse?.Message);
    }

    [Fact]
    public async Task AssignAssetLifeCycleToHolder_IfUserDontExist_NewUserIsAdded()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        Guid NEW_ASSETHOLDER_ID = new("dfe7639b-27d5-41b8-9926-43eba7f7e408");

        var asset = new AssignAssetToUser { UserId = NEW_ASSETHOLDER_ID, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetTwo}/customer/{_customerId}/assign";
        var response = await httpClient.PostAsync(requestUri, JsonContent.Create(asset));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var assignedAsset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.NotNull(assignedAsset);
        Assert.Equal(assignedAsset?.AssetHolderId, NEW_ASSETHOLDER_ID);
    }

    [Fact]
    public async Task GetDisposeSetting()
    {
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/dispose-setting";
        _testOutputHelper.WriteLine(requestUri);
        var setting = await _httpClient.GetFromJsonAsync<DisposeSetting>(requestUri);
        Assert.True(setting != null);
    }
    [Fact]
    public async Task GetDisposeSetting_ReturnNull()
    {
        var requestUri = $"/api/v1/Assets/customers/{Guid.NewGuid()}/dispose-setting";
        _testOutputHelper.WriteLine(requestUri);
        var setting = await _httpClient.GetFromJsonAsync<DisposeSetting>(requestUri);
        Assert.NotNull(setting);
    }

    [Fact]
    public async Task CreateDisposeSetting()
    {
        var newSettings = new NewDisposeSetting { CallerId = _callerId };
        var customerId = Guid.NewGuid();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{customerId}/dispose-setting";
        _testOutputHelper.WriteLine(requestUri);
        var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);

        var setting =
            await _httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{customerId}/dispose-setting");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    [Fact]
    public async Task AddReturnLocation_CustomerSettingNotExist()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var newSettings = new NewReturnLocation
        {
            Name = "Name", ReturnDescription = "Description", LocationId = Guid.NewGuid(), CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerIdTwo}/return-location";
        _testOutputHelper.WriteLine(requestUri);
        await httpClient.PostAsJsonAsync(requestUri, newSettings);
        var disposeSetting =
            await httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerIdTwo}/dispose-setting");

        Assert.True(disposeSetting!.ReturnLocations.Count == 1);
        Assert.True(disposeSetting!.ReturnLocations.FirstOrDefault()!.LocationId == newSettings.LocationId);
    }

    [Fact]
    public async Task AddReturnLocation()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var newSettings = new NewReturnLocation
        {
            Name = "Name", ReturnDescription = "Description", LocationId = Guid.NewGuid(), CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-location";
        _testOutputHelper.WriteLine(requestUri);
        await httpClient.PostAsJsonAsync(requestUri, newSettings);
        var disposeSetting =
            await httpClient.GetFromJsonAsync<DisposeSetting>(
                $"/api/v1/Assets/customers/{_customerId}/dispose-setting");

        Assert.True(disposeSetting!.ReturnLocations.Count == 2);
        Assert.Contains(disposeSetting!.ReturnLocations, x => x.LocationId == newSettings.LocationId);
    }

    [Fact]
    public async Task UpdateReturnLocation()
    {
        var updatedLocationId = Guid.Parse("d67e9568-f9c5-4180-8de1-341179748fe6");
        var newSettings = new NewReturnLocation
        {
            Name = "Name", ReturnDescription = "Description", LocationId = Guid.NewGuid(), CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-location";
        _testOutputHelper.WriteLine(requestUri);
        var added = await _httpClient.PostAsJsonAsync(requestUri, newSettings);
        var updateLocation = await added.Content.ReadFromJsonAsync<ReturnLocation>();
        var updateData = new NewReturnLocation
        {
            Name = "Name", ReturnDescription = "Description", LocationId = updatedLocationId
        };
        var response =
            await _httpClient.PutAsJsonAsync(
                $"/api/v1/Assets/customers/{_customerId}/return-location/{updateLocation!.Id}", updateData);

        var updatedSetting = await response.Content.ReadFromJsonAsync<ReturnLocation>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(updatedSetting!.Id == updateLocation.Id);
        Assert.True(updatedSetting.LocationId == updatedLocationId);
    }

    [Fact]
    public async Task RemoveReturnLocation()
    {
        var newSettings = new NewReturnLocation
        {
            Name = "Name", ReturnDescription = "Description", LocationId = Guid.NewGuid(), CallerId = _callerId
        };
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/return-location";
        _testOutputHelper.WriteLine(requestUri);
        var added = await _httpClient.PostAsJsonAsync(requestUri, newSettings);
        var addedLocations = await added.Content.ReadFromJsonAsync<ReturnLocation>();

        var response =
            await _httpClient.DeleteAsync(
                $"/api/v1/Assets/customers/{_customerId}/return-location/{addedLocations!.Id}");

        var updatedSetting = await response.Content.ReadFromJsonAsync<IList<ReturnLocation>>();

        Assert.True(updatedSetting!.Count == 1);
    }

    [Fact]
    public async Task GetCustomerAssetsCount()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var options = new FilterOptionsForAsset();
        var json = JsonSerializer.Serialize(options);

        var requestGet = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var responseGet = await httpClient.GetAsync(requestGet);
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);


        var assetsCounter = await responseGet.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(2, assetsCounter?.NonPersonal?.InUse);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Active);
        Assert.Equal(1, assetsCounter?.NonPersonal?.InputRequired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Available);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Expired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Repair);

        Assert.Equal(1, assetsCounter?.NonPersonal?.ExpiresSoon);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingRecycle);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingReturn);

        Assert.Equal(2, assetsCounter?.Personal?.InUse);
        Assert.Equal(0, assetsCounter?.Personal?.Active);
        Assert.Equal(0, assetsCounter?.Personal?.InputRequired);
        Assert.Equal(0, assetsCounter?.Personal?.Available);
        Assert.Equal(0, assetsCounter?.Personal?.Expired);
        Assert.Equal(0, assetsCounter?.Personal?.Repair);

        Assert.Equal(0, assetsCounter?.Personal?.ExpiresSoon);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);

        Assert.Empty(assetsCounter?.Departments);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_WithFilters()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        IList<Guid?> deprartments = new List<Guid?> { _departmentId };
        IList<AssetLifecycleStatus> status =
            new List<AssetLifecycleStatus> { AssetLifecycleStatus.Active, AssetLifecycleStatus.Available };

        var filterOptions = new FilterOptionsForAsset { Status = status, Department = deprartments };
        var json = JsonSerializer.Serialize(filterOptions);

        var requestGet = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var responseGet = await httpClient.GetAsync(requestGet);
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);


        var assetsCounter = await responseGet.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);

        Assert.NotNull(assetsCounter?.Departments);
        Assert.Equal(1, assetsCounter?.Departments?.Count);
        Assert.Equal(_departmentId, assetsCounter?.Departments[0]?.DepartmentId);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Available);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.NonPersonal?.Available);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_NotValidGuidForDepartment()
    {
        var json = "{\"Department\":[\"6244c47b-fcb3-4ea1-21-0\"],\"Status\":[0,3],\"Category\":null,\"Label\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("GetCustomerAssetsCount returns JsonException with message $.Department[0]",
            response.Content.ReadAsStringAsync().Result);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_EmptyGuidForDepartment()
    {
        var json =
            "{\"Department\":[\"00000000-0000-0000-0000-000000000000\"],\"Status\":[0,3],\"Category\":null,\"Label\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(0, assetsCounter?.NonPersonal?.InUse);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Active);
        Assert.Equal(0, assetsCounter?.NonPersonal?.InputRequired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Available);
        Assert.Equal(0, assetsCounter?.Personal?.InUse);
        Assert.Equal(0, assetsCounter?.Personal?.Active);
        Assert.Equal(0, assetsCounter?.Personal?.InputRequired);
        Assert.Equal(0, assetsCounter?.Personal?.Available);
        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);


        Assert.NotNull(assetsCounter?.Departments);
        Assert.Equal(0, assetsCounter?.Departments?.Count);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_EmptyGuidAndNot_Department()
    {
        var json =
            "{\"Department\":[\"00000000-0000-0000-0000-000000000000\",\"6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72\"],\"Status\":[0,3],\"Category\":null,\"Label\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);

        Assert.NotNull(assetsCounter?.Departments);
        Assert.Equal(1, assetsCounter?.Departments?.Count);
        Assert.Equal(_departmentId, assetsCounter?.Departments[0]?.DepartmentId);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Available);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.NonPersonal?.Available);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_NotValidStatus()
    {
        var json =
            "{\"Department\":[\"4fb46e5b-d29e-4de5-83a2-3d93bcd7a5cd\"],\"Status\":[15,20],\"Category\":null,\"Label\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Not a valid asset status",
            errorResponse?.Message);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_EmptyList()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var json = "{\"Department\":[],\"Status\":[],\"Category\":null,\"Label\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await httpClient.GetAsync(request);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();

        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(2, assetsCounter?.NonPersonal?.InUse);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Active);
        Assert.Equal(1, assetsCounter?.NonPersonal?.InputRequired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Available);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Expired);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Repair);

        Assert.Equal(1, assetsCounter?.NonPersonal?.ExpiresSoon);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingRecycle);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingReturn);

        Assert.Equal(2, assetsCounter?.Personal?.InUse);
        Assert.Equal(0, assetsCounter?.Personal?.Active);
        Assert.Equal(0, assetsCounter?.Personal?.InputRequired);
        Assert.Equal(0, assetsCounter?.Personal?.Available);
        Assert.Equal(0, assetsCounter?.Personal?.Expired);
        Assert.Equal(0, assetsCounter?.Personal?.Repair);

        Assert.Equal(0, assetsCounter?.Personal?.ExpiresSoon);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
        Assert.Equal(0, assetsCounter?.Personal?.PendingReturn);

        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);
        Assert.Empty(assetsCounter?.Departments);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_WithUserId()
    {
        var json =
            "{\"Department\":null,\"Status\":null,\"Category\":null,\"Label\":null,\"UserId\":\"6d16a4cb-4733-44de-b23b-0eb9e8ae6590\"}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(2, assetsCounter?.UsersPersonalAssets);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_WithNoUserId()
    {
        var json = "{\"Department\":null,\"Status\":null,\"Category\":null,\"Label\":null,\"UserId\":null}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);
    }

    [Fact]
    public async Task GetCustomerAssetsCount_EmptyGuid()
    {
        var json =
            "{\"Department\":null,\"Status\":null,\"Category\":null,\"Label\":null,\"UserId\":\"00000000-0000-0000-0000-000000000000\"}";

        var request = $"/api/v1/Assets/customers/{_customerId}/assets-counter/?filter={json}";
        var response = await _httpClient.GetAsync(request);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<CustomerAssetsCounter>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(_customerId, assetsCounter?.OrganizationId);
        Assert.Equal(0, assetsCounter?.UsersPersonalAssets);
    }

    [Fact]
    public async Task DeactivateAssetLifecycleStatus_ChangeStatus_NoLifecycle_LifecycleState()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetSeven}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetSeven, assetLifeCycle?.Id);


        Assert.Equal(AssetLifecycleStatus.Recycled, assetLifeCycle?.AssetStatus);
        var request = $"/api/v1/Assets/customers/{_customerId}/deactivate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetSeven }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(AssetLifecycleStatus.Inactive, assetsCounter?[0].AssetStatus);
    }
    [Fact]
    public async Task DeactivateAssetLifecycleStatus_MappingImeis()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetOne, assetLifeCycle?.Id);
        Assert.Equal(1, assetLifeCycle?.Imei.Count);
        Assert.All(assetLifeCycle.Imei, imei => Assert.Equal(500119468586675, imei));

        var request = $"/api/v1/Assets/customers/{_customerId}/deactivate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetOne }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsCounter);
        Assert.All(assetsCounter, assetLifecycle => Assert.Equal(1, assetLifecycle.Imei.Count));
        Assert.All(assetsCounter, assetLifecycle => Assert.All(assetLifecycle.Imei, imei => Assert.Equal(500119468586675, imei)));
    }


    [Fact]
    public async Task ActivateAssetLifecycleStatus_ChangeStatus_NoLifecycle_LifecycleState()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetSeven}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetSeven, assetLifeCycle?.Id);


        Assert.Equal(AssetLifecycleStatus.Recycled, assetLifeCycle?.AssetStatus);
        var request = $"/api/v1/Assets/customers/{_customerId}/activate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetSeven }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(AssetLifecycleStatus.Active, assetsCounter?[0].AssetStatus);
    }

    [Fact]
    public async Task ActivateAssetLifecycleStatus_ChangeStatus_BOYD_LifecycleState()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetNine}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetNine, assetLifeCycle?.Id);


        Assert.Equal(AssetLifecycleStatus.InputRequired, assetLifeCycle?.AssetStatus);
        var request = $"/api/v1/Assets/customers/{_customerId}/activate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetNine }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(AssetLifecycleStatus.Active, assetsCounter?[0].AssetStatus);
    }


    [Fact]
    public async Task DeactivateAssetLifecycleStatus_ChangeStatus_BOYD_LifecycleState()
    {
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetNine}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetNine, assetLifeCycle?.Id);


        Assert.Equal(AssetLifecycleStatus.InputRequired, assetLifeCycle?.AssetStatus);
        var request = $"/api/v1/Assets/customers/{_customerId}/deactivate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetNine }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsCounter);
        Assert.Equal(AssetLifecycleStatus.Inactive, assetsCounter?[0].AssetStatus);
    }


    [Fact]
    public async Task ActivateAssetLifecycleStatus_NotNoLifecycleOrBOYD_NoChangeOnStatus()
    {
        var request = $"/api/v1/Assets/customers/{_customerId}/activate";
        var body = new ChangeAssetStatus
        {
            AssetLifecycleId = new List<Guid> { _assetEight, _assetFive, _assetTwo, _assetThree },
            CallerId = _callerId
        };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.All(assetsCounter, m => Assert.NotEqual(AssetLifecycleStatus.Active, m.AssetStatus));
    }

    [Fact]
    public async Task DeactivateAssetLifecycleStatus_NotNoLifecycleOrBOYD_NoChangeOnStatus()
    {
        var request = $"/api/v1/Assets/customers/{_customerId}/deactivate";
        var body = new ChangeAssetStatus
        {
            AssetLifecycleId = new List<Guid> { _assetEight, _assetFive, _assetTwo, _assetThree },
            CallerId = _callerId
        };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.All(assetsCounter, m => Assert.NotEqual(AssetLifecycleStatus.Inactive, m.AssetStatus));
    }

    [Fact]
    public async Task ActivateAssetLifecycleStatus_EmptyGuidList()
    {
        var request = $"/api/v1/Assets/customers/{_customerId}/activate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid>(), CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetsCounter = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.Equal(0, assetsCounter?.Count);
    }

    [Fact]
    public async Task
        RepairCompleted_MultipleRequest_ShouldNotThrowError_ShouldNotThrowErrorButShouldGetorKeepStatusRepair_AndSwapIfNewImei()
    {
        var request = $"/api/v1/Assets/{_assetFour}/repair-completed";

        var swapImei = "524715417699766";
        var body = new AssetLifeCycleRepairCompleted { CallerId = _callerId, NewImei = new List<string> { swapImei } };


        var response = await _httpClient.PutAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var response_two = await _httpClient.PutAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response_two.StatusCode);
        var response_three = await _httpClient.PutAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response_three.StatusCode);
        var response_four = await _httpClient.PutAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response_four.StatusCode);
        var response_five = await _httpClient.PutAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response_five.StatusCode);

        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetFour}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetFour, assetLifeCycle?.Id);
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifeCycle?.AssetStatus);
        Assert.All(assetLifeCycle?.Imei, m => Assert.Equal(swapImei, m.ToString()));
    }

    [Fact]
    public async Task RepairCompleted_MultipleRequest_ShouldNotThrowErrorButShouldGetorKeepStatusRepair()
    {
        var uri = $"/api/v1/Assets/{_assetOne}/send-to-repair";

        var response = await _httpClient.PatchAsync(uri, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var response_two = await _httpClient.PatchAsync(uri, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response_two.StatusCode);
        var response_four = await _httpClient.PatchAsync(uri, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response_four.StatusCode);
        var response_five = await _httpClient.PatchAsync(uri, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response_five.StatusCode);
        var response_three = await _httpClient.PatchAsync(uri, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response_three.StatusCode);

        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetOne, assetLifeCycle?.Id);
        Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycle?.AssetStatus);
    }

    [Fact]
    public async Task UpdateAsset_MacAddress()
    {
        var macAddress = "01-23-45-67-89-AB";

        var request = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/update";

        var body = new UpdateAsset { MacAddress = macAddress };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(macAddress, asset?.MacAddress);


        var url = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}";
        var getAsset = await _httpClient.GetFromJsonAsync<API.ViewModels.Asset>(url);
        Assert.Equal(macAddress, getAsset?.MacAddress);
    }

    [Fact]
    public async Task UpdateAsset_MacAddress_notValid()
    {
        //Arrange
        var macAddress = "0123456789ab";
        var request = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/update";

        //Act
        var body = new UpdateAsset { MacAddress = macAddress };
        var response = await _httpClient.PostAsJsonAsync(request, body);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"Invalid mac address {macAddress}", errorResponse?.Message);
    }

    [Theory]
    [InlineData("01-23-45-67-89-AB", "01-23-45-67-89-AB")] //true
    [InlineData("01:23:45:67:89:AB", "01:23:45:67:89:AB")] //true
    [InlineData("0123.4567.89AB", "0123.4567.89AB")] //true
    [InlineData(" ", "01:23:00:67:89:AB")]
    public async Task UpdateAsset_ValidateMacAddress(string macAddress, string excpected)
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var request = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/update";

        var body = new UpdateAsset { MacAddress = macAddress };
        var response = await httpClient.PostAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(excpected, asset?.MacAddress);
    }

    [Theory]
    [InlineData("01-23-45-67-89-A�")] //false
    [InlineData("01-23-45-AB")] //false
    [InlineData("0123456789ab")] //false - missing separators (hyphen (-), colon(:) or dot (.)
    public async Task UpdateAsset_InValidateMacAddress(string macAddress)
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var request = $"/api/v1/Assets/{_assetFive}/customers/{_customerId}/update";

        var body = new UpdateAsset { MacAddress = macAddress };
        var response = await httpClient.PostAsJsonAsync(request, body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAsset_FilterOptions_IsPersonal()
    {
        var filterOptions = new FilterOptionsForAsset { IsPersonal = true };

        var json = JsonSerializer.Serialize(filterOptions);


        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(2, pagedAssetList!.Items.Count);
    }

    [Fact]
    public async Task GetAsset_FilterOptions_IsActiveState()
    {
        var filterOptions = new FilterOptionsForAsset { IsActiveState = true };

        var json = JsonSerializer.Serialize(filterOptions);


        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

        // Assert
        Assert.Equal(12, pagedAssetList!.Items.Count);
    }

    [Fact]
    public async Task PatchAsset_AssignToUser_AssetInputRequired()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var assignment = new AssignAssetToUser { UserId = _user, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetNine}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(_user, asset?.AssetHolderId);
        Assert.Null(asset?.ManagedByDepartmentId);
        Assert.True(asset?.IsPersonal);
    }

    [Fact]
    public async Task PatchAsset_AssignToUser_WhenUserIsNotInUserTable_AssetInputRequired()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var userId = Guid.NewGuid();
        var assignment = new AssignAssetToUser { UserId = userId, CallerId = _callerId };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(userId, asset?.AssetHolderId);
        Assert.Null(asset?.ManagedByDepartmentId);
        Assert.True(asset?.IsPersonal);
    }

    [Fact]
    public async Task PatchAsset_AssignToUser_WhenUserIsAssignedToADepartment()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var userId = Guid.NewGuid();
        var assignment = new AssignAssetToUser
        {
            UserId = userId, CallerId = _callerId, UserAssigneToDepartment = _departmentId
        };
        var requestUri = $"/api/v1/Assets/{_assetOne}/customer/{_customerId}/assign";
        var response = await _httpClient.PostAsJsonAsync(requestUri, assignment);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var asset = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.Equal(userId, asset?.AssetHolderId);
        Assert.NotNull(asset?.ManagedByDepartmentId);
        Assert.Equal(_departmentId, asset?.ManagedByDepartmentId);
        Assert.True(asset?.IsPersonal);
    }

    [Fact]
    public async Task GetAssetlifecycle_CheckingViewmodelValues()
    {
        //Get one asset check that it has imei
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetOne, assetLifeCycle?.Id);
        Assert.Equal(1, assetLifeCycle?.Imei.Count);
        Assert.Equal(_user, assetLifeCycle?.AssetHolderId);

        //Activate assets check that imei is same
        var request = $"/api/v1/Assets/customers/{_customerId}/activate";
        var body = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { _assetOne }, CallerId = _callerId };
        var response = await _httpClient.PostAsJsonAsync(request, body);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assets = await response.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assets);
        Assert.Equal(1, assets?[0].Imei.Count);
        Assert.Equal(_user, assets?[0].AssetHolderId);

        //Get labels
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/labels";
        var labels = await _httpClient.GetFromJsonAsync<List<Label>>(requestUri);

        //Assigne labels to asset
        var requestlabels = $"/api/v1/Assets/customers/{_customerId}/labels/assign";
        var labelsBody = new AssetLabels
        {
            AssetGuids = new List<Guid> { _assetOne },
            LabelGuids = labels.Select(a => a.Id).ToList(),
            CallerId = _callerId
        };
        var responseLabels = await _httpClient.PostAsJsonAsync(requestlabels, labelsBody);
        var assetsLabels = await responseLabels.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();

        //deactivate
        var requestDeactivate = $"/api/v1/Assets/customers/{_customerId}/deactivate";
        var bodyDeactivate = new ChangeAssetStatus
        {
            AssetLifecycleId = new List<Guid> { _assetOne }, CallerId = _callerId
        };
        var responseDeactivate = await _httpClient.PostAsJsonAsync(requestDeactivate, bodyDeactivate);

        Assert.Equal(HttpStatusCode.OK, responseDeactivate.StatusCode);
        var assetsDeactivated = await responseDeactivate.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsDeactivated);
        Assert.Equal(1, assetsDeactivated?[0].Imei.Count);
        Assert.Equal(_user, assetsDeactivated?[0].AssetHolderId);
        Assert.Equal(2, assetsDeactivated?[0].Labels.Count);


        //Get asset
        var requestGetAssetLifeCycle1 = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle1 = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle1.StatusCode);
        var assetLifeCycle1 = await responseGetAssetLifeCycle1.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(2, assetLifeCycle1?.Labels.Count);
    }

    [Fact]
    public async Task AssignLabels_CheckingViewmodelValues()
    {
        //Get one asset check that it has imei
        var request = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var response = await _httpClient.GetAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assetLifeCycleBefore = await response.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(0, assetLifeCycleBefore?.Labels.Count);

        //Get labels
        var requestUri = $"/api/v1/Assets/customers/{_customerId}/labels";
        var labels = await _httpClient.GetFromJsonAsync<List<Label>>(requestUri);

        //Assigne labels to asset
        var requestlabels = $"/api/v1/Assets/customers/{_customerId}/labels/assign";
        var labelsBody = new AssetLabels
        {
            AssetGuids = new List<Guid> { _assetOne },
            LabelGuids = labels.Select(a => a.Id).ToList(),
            CallerId = _callerId
        };
        var responseLabels = await _httpClient.PostAsJsonAsync(requestlabels, labelsBody);
        var assetsLabels = await responseLabels.Content.ReadFromJsonAsync<IList<API.ViewModels.Asset>>();
        Assert.NotNull(assetsLabels);
        Assert.Equal(_assetOne, assetsLabels[0]?.Id);
        Assert.Equal(1, assetsLabels[0]?.Imei.Count);
        Assert.Equal(_user, assetsLabels[0]?.AssetHolderId);
        Assert.Equal(2, assetsLabels[0]?.Labels.Count);

        //Get asset
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetOne, assetLifeCycle?.Id);
        Assert.Equal(1, assetLifeCycle?.Imei.Count);
        Assert.Equal(_user, assetLifeCycle?.AssetHolderId);
        Assert.Equal(2, assetLifeCycle?.Labels.Count);
    }

    [Fact]
    public async Task GetAssetsForCustomer_IsActiveState_ShouldBeTrue()
    {
        var filterOptions = new FilterOptionsForAsset { IsActiveState = true };

        var json = JsonSerializer.Serialize(filterOptions);


        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

        // Act
        var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);
        var items = pagedAssetList?.Items;

        // Assert
        Assert.NotNull(items);
        Assert.All(items, m => Assert.True(m.IsActiveState));
    }

    [Fact]
    public async Task GetAsset_IsActiveState_ShouldBeTrue()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var url = $"/api/v1/Assets/{_assetSix}/customers/{_customerId}";
        var asset = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(url);
        Assert.True(asset?.IsActiveState);
    }
    [Fact]
    public async Task GetAsset_WithFilter()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var userId = _user.ToString();
        var filterOptions = new FilterOptionsForAsset { UserId = userId };

        var json = JsonSerializer.Serialize(filterOptions);
        

        var url = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}?filterOptions={json}";
        var asset = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(url);
        Assert.Equal(_user, asset?.AssetHolderId);
        Assert.Equal(_assetOne, asset?.Id);
    }
    [Fact]
    public async Task GetAsset_Manager_FilterByDepartmentId()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
       
        var filterOptions = new FilterOptionsForAsset { Department = new List<Guid?> { _departmentId } };

        var json = JsonSerializer.Serialize(filterOptions);


        var url = $"/api/v1/Assets/{_assetSeven}/customers/{_customerId}?filterOptions={json}";
        var asset = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(url);
        Assert.Equal(_departmentId, asset?.ManagedByDepartmentId);
        Assert.Null(asset?.AssetHolderId);
        Assert.Equal(_assetSeven, asset?.Id);
    }
    [Fact]
    public async Task GetAsset_Manager_GetUserOfDepartmentAsset()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var filterOptions = new FilterOptionsForAsset { Department = new List<Guid?> { _departmentId } };

        var json = JsonSerializer.Serialize(filterOptions);
        var url = $"/api/v1/Assets/{_assetFour}/customers/{_customerId}?filterOptions={json}";
        var asset = await httpClient.GetFromJsonAsync<API.ViewModels.Asset>(url);
        Assert.Equal(_user, asset?.AssetHolderId);
        Assert.Equal(_departmentId, asset?.ManagedByDepartmentId);
        Assert.Equal(_assetFour, asset?.Id);
    }
    [Fact]
    public async Task GetAsset_Manager_NotFromManagersDepartment()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var filterOptions = new FilterOptionsForAsset { Department = new List<Guid?> { _departmentIdTwo } };

        var json = JsonSerializer.Serialize(filterOptions);
        var url = $"/api/v1/Assets/{_assetFour}/customers/{_customerId}?filterOptions={json}";
        var response = await httpClient.GetAsync(url);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    [Fact]
    public async Task RecycleAssetLifecycle_ChangeAssetStatus_ReturnsOK()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var recycleAssetLifecycle = new RecycleAssetLifecycle { AssetLifecycleStatus = 10, CallerId = _callerId};
        var url = $"/api/v1/Assets/{_assetOne}/recycle";
        var response = await _httpClient.PatchAsync(url, JsonContent.Create(recycleAssetLifecycle));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetOne}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

        Assert.NotNull(assetLifeCycle);
        Assert.Equal(_assetOne, assetLifeCycle?.Id);
        Assert.Equal(AssetLifecycleStatus.Recycled, assetLifeCycle?.AssetStatus);
    }
    [Fact]
    public async Task RecycleAssetLifecycle_InvalidStatus_ReturnsBadRequest()
    {
        //Arrange
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
        var recycleAssetLifecycle = new RecycleAssetLifecycle { AssetLifecycleStatus = 20, CallerId = _callerId };
        var url = $"/api/v1/Assets/{_assetOne}/recycle";
        //Act
        var response = await _httpClient.PatchAsync(url, JsonContent.Create(recycleAssetLifecycle));
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task CancelReturn_AssetLifecycleHasExpired_ReturnsOKAndAssetLifecycleStatusExpired()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Order a recycle status change
        var recycleAssetLifecycle = new RecycleAssetLifecycle { AssetLifecycleStatus = 10, CallerId = _callerId };
        var urlRecycleDevice = $"/api/v1/Assets/{_assetThree}/recycle";
        var responseRecycleDevice = await _httpClient.PatchAsync(urlRecycleDevice, JsonContent.Create(recycleAssetLifecycle));
        Assert.Equal(HttpStatusCode.OK, responseRecycleDevice.StatusCode);

        //Check the end date and if the asset lifecycle has gotten the status recycled
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetThree}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifecycleRecycled = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(DateTime.UtcNow.Date, assetLifecycleRecycled?.EndPeriod.GetValueOrDefault().Date);
        Assert.Equal(AssetLifecycleStatus.Recycled, assetLifecycleRecycled?.AssetStatus);

        //Cancel the recycle
        var url = $"/api/v1/Assets/{_assetThree}/cancel-return";
        var response = await httpClient.PatchAsync(url, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //Check that the asset status is expired since the end date of the asset lifecycles runtime is today
        var responseGetAssetLifeCycleAfter = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycleAfter.StatusCode);
        var assetLifecycleRecycledAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(DateTime.UtcNow.Date, assetLifecycleRecycledAfter?.EndPeriod.GetValueOrDefault().Date);
        Assert.Equal(AssetLifecycleStatus.Expired, assetLifecycleRecycledAfter?.AssetStatus);
    }
    [Fact]
    public async Task CancelReturn_TransactionalAssetLifecycleChangeToInUse_ReturnsOKAndAssetLifecycleStatusIsChangedToInUse()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Order a recycle status change
        var recycleAssetLifecycle = new RecycleAssetLifecycle { AssetLifecycleStatus = 10, CallerId = _callerId };
        var urlRecycleDevice = $"/api/v1/Assets/{_assetTwo}/recycle";
        var responseRecycleDevice = await _httpClient.PatchAsync(urlRecycleDevice, JsonContent.Create(recycleAssetLifecycle));
        Assert.Equal(HttpStatusCode.OK, responseRecycleDevice.StatusCode);

        //Check the end date and if the asset lifecycle has gotten the status recycled
        var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetTwo}/customers/{_customerId}";
        var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
        var assetLifecycleRecycled = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(AssetLifecycleStatus.Recycled, assetLifecycleRecycled?.AssetStatus);

        //Cancel the recycle
        var url = $"/api/v1/Assets/{_assetTwo}/cancel-return";
        var response = await httpClient.PatchAsync(url, JsonContent.Create(_callerId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //Check that the asset status is expired since the end date of the asset lifecycles runtime is today
        var responseGetAssetLifeCycleAfter = await _httpClient.GetAsync(requestGetAssetLifeCycle);
        Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycleAfter.StatusCode);
        var assetLifecycleRecycledAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
        Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, assetLifecycleRecycled?.EndPeriod.GetValueOrDefault().Date);
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifecycleRecycledAfter?.AssetStatus);
    }
    [Fact]
    public async Task CancelReturn_AssetLifecycleStatusIsNotValidForCancelingAReturn_ThrowsExceptionAndReturnsBadRequest()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        //Cancel the an asset that is Available
        var url = $"/api/v1/Assets/{_assetTwo}/cancel-return";
        var response = await httpClient.PatchAsync(url, JsonContent.Create(_callerId));
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"Invalid asset lifecycle status: Available for completing return.", errorResponse?.Message);
    }
    [Fact]
    public async Task GetAssetsForUser_IncludeAssetValues()
    {
        var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);

        var url = $"/api/v1/Assets/customers/{_customerId}/users/{_user}";
        var response = await httpClient.GetAsync(url);
        var assets = await response.Content.ReadFromJsonAsync<IEnumerable<API.ViewModels.Asset>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(assets);
        Assert.All(assets, item => Assert.NotNull(item.ProductName));
        Assert.All(assets, item => Assert.Equal(1,item.Imei.Count));
    }
}