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

namespace Asset.IntegrationTests.Controllers
{
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
        private readonly Guid _assetFive;
        private readonly Guid _assetEight;


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
            _assetEight = factory.ASSETLIFECYCLE_EIGHT_ID;
            _assetFive = factory.ASSETLIFECYCLE_FIVE_ID;
            _customerIdTwo = factory.COMPANY_ID_TWO;
            _factory = factory;
        }

        [Fact]
        public async Task GetAssetsForCustomer()
        {
            int[] category = new int[] { 1,2 };
            IList<Guid?> depts = new List<Guid?> { new Guid("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72") };
            IList<AssetLifecycleStatus> status = new List<AssetLifecycleStatus>{AssetLifecycleStatus.Active ,
            AssetLifecycleStatus.Recycled};
            Guid[] labels = new Guid[] { new Guid("cab4bb77-3471-4ab3-ae5e-2d4fce450f36"),
            new Guid("dab4bb77-3471-4ab3-ae5e-2d4fce450f37")};


            var filterOptions = new FilterOptionsForAsset
            {
                Status = status,
                Label = labels,
                Department = depts,
                Category = category
            };

            var json = JsonSerializer.Serialize(filterOptions);

            var httpClient = _factory.CreateClientWithDbSetup(AssetTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/Assets/customers/{_customerId}?filterOptions={json}";

            // Act
            var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

            // Assert
            Assert.Equal(2, pagedAssetList!.Items.Count);
            Assert.Equal(DateTime.UtcNow.Date, pagedAssetList!.Items[0]!.CreatedDate.Date);
            Assert.Equal(DateTime.UtcNow.Date, pagedAssetList!.Items[1]!.CreatedDate.Date);
        }

        [Fact]
        public async Task GetCustomerItemCount_ForCustomer()
        {
            var requestUri = $"/api/v1/Assets/customers/{_customerId}/count";
            _testOutputHelper.WriteLine(requestUri);
            var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
            Assert.Equal(6, count);
        }

        [Fact]
        public async Task GetCustomerItemCount_ForCustomerWithDepartment()
        {
            var requestUri = $"/api/v1/Assets/customers/{_customerId}/count?departmentId={_departmentId}";
            _testOutputHelper.WriteLine(requestUri);
            var count = await _httpClient.GetFromJsonAsync<int>(requestUri);
            Assert.Equal(3, count);
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
            Assert.Equal(2, count);
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

            FilterOptionsForAsset filter = new FilterOptionsForAsset();
            string json = JsonSerializer.Serialize(filter);
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

            FilterOptionsForAsset asset1 = new FilterOptionsForAsset();
            string json = JsonSerializer.Serialize(asset1);
            var pagedAssetList =
                await httpClient.GetFromJsonAsync<PagedAssetList>($"/api/v1/Assets/customers/{customerId}?filterOptions={json}");

            Assert.NotNull(pagedAssetList);
            Assert.Equal(7, pagedAssetList!.TotalItems);
            Assert.All(pagedAssetList.Items, m => Assert.Equal(data.DepartmentId, m.ManagedByDepartmentId));
            Assert.All(pagedAssetList.Items, m => Assert.Null(m.AssetHolderId));
        }

        [Fact]
        public async Task GetLifeCycleSetting()
        {
            var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
            _testOutputHelper.WriteLine(requestUri);
            var setting = await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(requestUri);
            Assert.Equal(setting!.FirstOrDefault()!.CustomerId, _customerId);
            Assert.Equal(1, setting!.Count);
        }

        [Fact]
        public async Task CreateLifeCycleSetting()
        {
            var newSettings =
                new NewLifeCycleSetting { AssetCategoryId = 1, BuyoutAllowed = true, Runtime = 12, CallerId = _callerId };
            var customerId = Guid.NewGuid();
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
            var requestUri = $"/api/v1/Assets/customers/{customerId}/lifecycle-setting";
            _testOutputHelper.WriteLine(requestUri);
            var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newSettings);

            var setting =
                await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(
                    $"/api/v1/Assets/customers/{customerId}/lifecycle-setting");

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.Equal(setting!.FirstOrDefault()!.CustomerId, customerId);
            Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.BuyoutAllowed ==
                        newSettings.BuyoutAllowed);
            Assert.True(setting!.FirstOrDefault(x => x.AssetCategoryId == newSettings.AssetCategoryId)!.Runtime ==
                        newSettings.Runtime);
        }

        [Fact]
        public async Task GetAvailableAssetsForCustomer()
        {
            FilterOptionsForAsset options = new FilterOptionsForAsset();
            string json = JsonSerializer.Serialize(options);
            var requestUri = $"/api/v1/Assets/customers/{_customerId}?page=1&limit=1000&status=9&filterOptions={json}";
            _testOutputHelper.WriteLine(requestUri);
            var pagedAsset = await _httpClient.GetFromJsonAsync<PagedAssetList>(requestUri);

            Assert.True(pagedAsset!.TotalItems == 7);
            Assert.True(pagedAsset.Items.Count == 7);
            Assert.True(pagedAsset.Items.Count(x => x.AssetStatus == AssetLifecycleStatus.InUse) == 4);
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
                Personal = true,
                UserId = Guid.Parse(userId),
                DepartmentId = Guid.Parse(deptId),
                CallerId = _callerId
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
            var reAssignAsset = new ReAssignAsset
            {
                UserId = _user, DepartmentId = _departmentIdTwo, CallerId = _callerId
            };

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
                Personal = false,
                UserId = Guid.Parse(userId),
                DepartmentId = Guid.Parse(deptId),
                CallerId = _callerId
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
            var newSettings =
                new NewLifeCycleSetting { AssetCategoryId = 1, BuyoutAllowed = false, CallerId = _callerId };
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(newSettings));
            var requestUri = $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting";
            _testOutputHelper.WriteLine(requestUri);
            await _httpClient.PutAsJsonAsync(requestUri, newSettings);
            var setting =
                await _httpClient.GetFromJsonAsync<IList<LifeCycleSetting>>(
                    $"/api/v1/Assets/customers/{_customerId}/lifecycle-setting");

            Assert.Equal(setting!.FirstOrDefault()!.CustomerId, _customerId);
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
                new() { Color = LabelColor.Blue, Text = "Label1" },
                new() { Color = LabelColor.Green, Text = "Label2" }
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
            FilterOptionsForAsset asset1 = new FilterOptionsForAsset();
            var filterOptions = JsonSerializer.Serialize(asset1);
            var requestAllAssets = $"/api/v1/Assets/customers/{_customerId}?filterOptions={filterOptions}";
            var pagedAssetList = await httpClient.GetFromJsonAsync<PagedAssetList>(requestAllAssets);
            Assert.Equal(7, pagedAssetList!.Items.Count);
            var assetOneFromResponse = pagedAssetList.Items.FirstOrDefault(i => i.Id == _assetOne);
            Assert.Equal(_assetOne, assetOneFromResponse!.Id);
            Assert.Equal(2, assetOneFromResponse.Labels.Count);
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
        public async Task AssetLifeCycleChangeStatusToRepair()
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
            var assetLifeCycleAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();

            Assert.Equal(AssetLifecycleStatus.Repair, assetLifeCycleAfter?.AssetStatus);
        }
        [Fact]
        public async Task AssetLifeCycleChangeStatusToRepair_NotValidGuidForAsset_ReturnsNotFound()
        {
            var NOT_VALID_GUID = Guid.NewGuid();
            var requestUri = $"/api/v1/Assets/{NOT_VALID_GUID}/send-to-repair";
            var response = await _httpClient.PatchAsync(requestUri, JsonContent.Create(_callerId));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal($"AssetLifeCycleChangeStatusToRepair returns ResourceNotFoundException with assetLifecycleId {NOT_VALID_GUID}", response.Content.ReadAsStringAsync().Result);
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



            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
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
            var assetLifeCycleAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
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


            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
            {
                Discarded = false,
                CallerId = _callerId,
                NewSerialNumber = "12345678975212"
            };
            var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
            var response = await httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //After
            var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycle);
            var assetLifeCycleAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
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


            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
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
            var assetLifeCycleAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
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



            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
            {
                Discarded = true,
                CallerId = _callerId
            };
            var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
            var response = await httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //After
            var responseGetAssetLifeCycleAfter = await httpClient.GetAsync(requestGetAssetLifeCycle);
            var assetLifeCycleAfter = await responseGetAssetLifeCycleAfter.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
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
            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
            {
                Discarded = false,
                CallerId = _callerId,
                NewImei = new List<string> { INVALID_IMEI }
            };
            var requestUri = $"/api/v1/Assets/{_assetEight}/repair-completed";
            var response = await _httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal($"AssetLifeCycleRepairCompleted returns InvalidAssetDataException with assetLifecycleId {_assetEight} with message: Invalid imei: {INVALID_IMEI}", response.Content.ReadAsStringAsync().Result);
        }
        [Fact]
        public async Task AssetLifeCycleRepairCompleted_NewAssetShouldBeMade_InvalidStatusBadRequest()
        {
            var requestGetAssetLifeCycle = $"/api/v1/Assets/{_assetTwo}/customers/{_customerId}";
            var responseGetAssetLifeCycle = await _httpClient.GetAsync(requestGetAssetLifeCycle);
            Assert.Equal(HttpStatusCode.OK, responseGetAssetLifeCycle.StatusCode);
            var assetLifeCycle = await responseGetAssetLifeCycle.Content.ReadFromJsonAsync<API.ViewModels.Asset>();
            Assert.NotNull(assetLifeCycle);
            Assert.Equal(_assetTwo, assetLifeCycle?.Id);
            Assert.Equal(AssetLifecycleStatus.Available, assetLifeCycle?.AssetStatus);

            var assetLifeCycleRepairCompleted = new AssetServices.ServiceModel.AssetLifeCycleRepairCompleted
            {
                Discarded = false,
                CallerId = _callerId,
            };
            var requestUri = $"/api/v1/Assets/{_assetTwo}/repair-completed";
            var response = await _httpClient.PutAsJsonAsync(requestUri, assetLifeCycleRepairCompleted);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal($"AssetLifeCycleRepairCompleted returns InvalidAssetDataException with assetLifecycleId {_assetTwo} with message: Asset life cycle has status Available", response.Content.ReadAsStringAsync().Result);
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
    }
}