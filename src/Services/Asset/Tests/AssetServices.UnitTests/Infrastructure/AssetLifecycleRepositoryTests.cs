using AssetServices.Models;
using Common.Enums;
using Common.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AssetServices.Infrastructure.Tests
{
    public class AssetLifecycleRepositoryTests
    {
        private readonly AssetsContext _dbContext;
        private readonly AssetLifecycleRepository _repository;

        private readonly Guid CUSTOMER1 = Guid.NewGuid();
        private readonly Guid CUSTOMER2 = Guid.NewGuid();
        private readonly Guid CUSTOMER3 = Guid.NewGuid();

        private readonly Guid USER1 = Guid.NewGuid();
        private readonly Guid USER2 = Guid.NewGuid();

        private readonly Guid DEPARTMENT1 = Guid.NewGuid();
        private readonly Guid DEPARTMENT2 = Guid.NewGuid();

        private readonly Guid CALLER_ID = Guid.NewGuid();


        public AssetLifecycleRepositoryTests()
        {
            _dbContext = new AssetsContext(new DbContextOptionsBuilder<AssetsContext>().UseSqlite($"Data Source={Guid.NewGuid()}.db").Options);
            _repository = new AssetLifecycleRepository(_dbContext, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            AdvancedSearchSeedData();
        }

        // As we are testing the repository itself and therefore want more granular control, we don't use the data-seeding from "AssetBaseTest" 
        private void AdvancedSearchSeedData()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Users
            var user1 = new User() { ExternalId = USER1, Name = "John Doe" };
            var user2 = new User() { ExternalId = USER2, Name = "Ole Normann" };

            // Assets
            var asset1 = new MobilePhone(Guid.NewGuid(), CALLER_ID, "LL6-dd5DaYY6", "Samsung", "Galaxy S20", new List<AssetImei>() { new AssetImei(334937775025511), new AssetImei(301514703166146) }, "39:C5:BB:C8:93:A2");
            var asset2 = new MobilePhone(Guid.NewGuid(), CALLER_ID, "9988/782", "Apple", "iPhone 8", new List<AssetImei>() { new AssetImei(530884107774486) }, "BC:CC:93:66:C8:7F");
            var asset3 = new MobilePhone(Guid.NewGuid(), CALLER_ID, "9988/783", "Samsung", "Galaxy S21", new List<AssetImei>() { new AssetImei(350053004089850) }, "28:0A:1F:16:CA:D6");
            var asset4 = new MobilePhone(Guid.NewGuid(), CALLER_ID, "8988/543", "Samsung", "Galaxy S21 Ultra", new List<AssetImei>() { new AssetImei(530739382092163) }, "D9:1F:14:87:7F:A5");
            var asset5 = new Tablet(Guid.NewGuid(), CALLER_ID, "SN:664-4337A1Y", "Lenovo", "Yoga Tab 13", new List<AssetImei>() { new AssetImei(869302915930937) }, string.Empty);
            var asset6 = new Tablet(Guid.NewGuid(), CALLER_ID, "SN:664-9977A1Y", "Lenovo", "Yoga Tab 13", new List<AssetImei>() { }, string.Empty);

            // Asset Lifecycles
            var assetLifecycle1 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER1, Alias = "alias_1", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional, PurchaseDate = DateTime.Parse("2022-01-15"), StartPeriod = DateTime.Parse("2022-01-15"), EndPeriod = DateTime.Parse("2023-01-15") };
            assetLifecycle1.AssignAsset(asset1, CALLER_ID);
            assetLifecycle1.AssignAssetLifecycleHolder(user1, null, CALLER_ID);

            var assetLifecycle2 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER1, Alias = "alias_2", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional, PurchaseDate = DateTime.Parse("2022-03-19"), StartPeriod = DateTime.Parse("2022-03-19"), EndPeriod = DateTime.Parse("2023-03-19") };
            assetLifecycle2.AssignAsset(asset2, CALLER_ID);
            assetLifecycle2.AssignAssetLifecycleHolder(contractHolderUser: null, DEPARTMENT1, CALLER_ID);

            var assetLifecycle3 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER2, Alias = "alias_3", AssetLifecycleStatus = AssetLifecycleStatus.Repair, AssetLifecycleType = LifecycleType.Transactional, PurchaseDate = DateTime.Parse("2022-03-19"), StartPeriod = DateTime.Parse("2022-03-19"), EndPeriod = DateTime.Parse("2023-03-19") };
            assetLifecycle3.AssignAsset(asset3, CALLER_ID);

            var assetLifecycle4 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER3, Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.Active, AssetLifecycleType = LifecycleType.NoLifecycle, PurchaseDate = DateTime.Parse("2021-12-27"), StartPeriod = DateTime.Parse("2021-12-27"), EndPeriod = DateTime.Parse("2022-12-27") };
            assetLifecycle4.AssignAsset(asset4, CALLER_ID);
            assetLifecycle4.AssignAssetLifecycleHolder(contractHolderUser: null, DEPARTMENT2, CALLER_ID);

            var assetLifecycle5 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER3, Alias = "alias_5", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.BYOD, PurchaseDate = DateTime.Parse("2021-12-28"), StartPeriod = DateTime.Parse("2021-12-28"), EndPeriod = DateTime.Parse("2022-12-28") };
            assetLifecycle5.AssignAsset(asset5, CALLER_ID);
            assetLifecycle5.AssignAssetLifecycleHolder(user2, null, CALLER_ID);

            var assetLifecycle6 = new AssetLifecycle(Guid.NewGuid()) { CustomerId = CUSTOMER3, Alias = "Different Alias 6", AssetLifecycleStatus = AssetLifecycleStatus.Repair, AssetLifecycleType = LifecycleType.BYOD, PurchaseDate = DateTime.Parse("2021-12-28"), StartPeriod = DateTime.Parse("2021-12-28"), EndPeriod = DateTime.Parse("2022-12-28") };
            assetLifecycle6.AssignAsset(asset6, CALLER_ID);
            assetLifecycle6.AssignAssetLifecycleHolder(null, null, CALLER_ID);

            // Persist
            _dbContext.Users.AddRange(user1, user2);
            _dbContext.Assets.AddRange(asset1, asset2, asset3, asset4, asset5, asset6);
            _dbContext.AssetLifeCycles.AddRange(assetLifecycle1, assetLifecycle2, assetLifecycle3, assetLifecycle4, assetLifecycle5, assetLifecycle6);
            _dbContext.SaveChanges();
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: No filters")]
        public async Task AdvancedSearch_NoOptionalFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters noParameters = new();
            var startWithResults = await _repository.AdvancedSearchAsync(noParameters, 1, 25, new());

            // Assert
            Assert.Equal(6, startWithResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Customer ID filter")]
        public async Task AdvancedSearch_CustomerIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { CustomerIds = new() { CUSTOMER1 } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { CustomerIds = new() { Guid.NewGuid() } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { CustomerIds = new() { CUSTOMER1, CUSTOMER2 } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { CustomerIds = new() { CUSTOMER1, CUSTOMER2, Guid.NewGuid(), Guid.NewGuid() } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(2, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(3, severalExistingIdsResults.Items.Count);
            Assert.Equal(3, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: User ID filter")]
        public async Task AdvancedSearch_UserIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { UserIds = new() { USER1 } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { UserIds = new() { Guid.NewGuid() } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { UserIds = new() { USER1, USER2 } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { UserIds = new() { USER1, USER2, Guid.NewGuid(), Guid.NewGuid() } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(1, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(2, severalExistingIdsResults.Items.Count);
            Assert.Equal(2, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Department ID filter")]
        public async Task AdvancedSearch_DepartmentIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { DepartmentIds = new() { DEPARTMENT1 } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { DepartmentIds = new() { Guid.NewGuid() } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { DepartmentIds = new() { DEPARTMENT1, DEPARTMENT2 } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { DepartmentIds = new() { DEPARTMENT1, DEPARTMENT2, Guid.NewGuid(), Guid.NewGuid() } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(1, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(2, severalExistingIdsResults.Items.Count);
            Assert.Equal(2, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Category ID filter")]
        public async Task AdvancedSearch_CategoryIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { AssetCategoryIds = new() { 2 } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { AssetCategoryIds = new() { 90 } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { AssetCategoryIds = new() { 1, 2 } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { AssetCategoryIds = new() { 1, 2, 90, 91 } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(2, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(6, severalExistingIdsResults.Items.Count);
            Assert.Equal(6, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Lifecycle Status ID filter")]
        public async Task AdvancedSearch_LifecycleStatusIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { AssetLifecycleStatusIds = new() { (int)AssetLifecycleStatus.Repair } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { AssetLifecycleStatusIds = new() { 90 } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { AssetLifecycleStatusIds = new() { (int)AssetLifecycleStatus.Repair, (int)AssetLifecycleStatus.InUse } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { AssetLifecycleStatusIds = new() { (int)AssetLifecycleStatus.Repair, (int)AssetLifecycleStatus.InUse, 90, 91 } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(2, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(5, severalExistingIdsResults.Items.Count);
            Assert.Equal(5, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Lifecycle Type ID filter")]
        public async Task AdvancedSearch_LifecycleTypeIdFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters oneExistingIdParameters = new() { AssetLifecycleTypeIds = new() { (int)LifecycleType.Transactional } };
            var oneExistingIdResults = await _repository.AdvancedSearchAsync(oneExistingIdParameters, 1, 25, new());

            SearchParameters oneNonExistingIdParameters = new() { AssetLifecycleTypeIds = new() { 90 } };
            var oneNonExistingIdResults = await _repository.AdvancedSearchAsync(oneNonExistingIdParameters, 1, 25, new());

            SearchParameters severalExistingIdsParameters = new() { AssetLifecycleTypeIds = new() { (int)LifecycleType.Transactional, (int)LifecycleType.BYOD } };
            var severalExistingIdsResults = await _repository.AdvancedSearchAsync(severalExistingIdsParameters, 1, 25, new());

            SearchParameters existingAndNonExistingIdsParameters = new() { AssetLifecycleTypeIds = new() { (int)LifecycleType.Transactional, (int)LifecycleType.BYOD, 90, 91 } };
            var existingAndNonExistingResults = await _repository.AdvancedSearchAsync(existingAndNonExistingIdsParameters, 1, 25, new());


            // Assert
            Assert.Equal(3, oneExistingIdResults.Items.Count);
            Assert.Equal(0, oneNonExistingIdResults.Items.Count);
            Assert.Equal(5, severalExistingIdsResults.Items.Count);
            Assert.Equal(5, existingAndNonExistingResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: IMEI filter")]
        public async Task AdvancedSearch_ImeiFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters startWithParameters = new() { Imei = "530", ImeiSearchType = StringSearchType.StartsWith };
            var startWithResults = await _repository.AdvancedSearchAsync(startWithParameters, 1, 25, new());

            SearchParameters containsParameters = new() { Imei = "530", ImeiSearchType = StringSearchType.Contains };
            var containsResults = await _repository.AdvancedSearchAsync(containsParameters, 1, 25, new());

            SearchParameters equalsParameters = new() { Imei = "530739382092163", ImeiSearchType = StringSearchType.Equals };
            var equalsResults = await _repository.AdvancedSearchAsync(equalsParameters, 1, 25, new());

            // Assert
            Assert.Equal(2, startWithResults.Items.Count);
            Assert.Equal(3, containsResults.Items.Count);
            Assert.Equal(1, equalsResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Serial-number filter")]
        public async Task AdvancedSearch_SerialNumberFilter_AsyncTest()
        {
            /*
             * Arrange
             */



            /*
             * Act
             */

            SearchParameters startWithParameters = new() { SerialNumber = "9988/", SerialNumberSearchType = StringSearchType.StartsWith };
            var startWithResults = await _repository.AdvancedSearchAsync(startWithParameters, 1, 25, new());

            SearchParameters containsParameters = new() { SerialNumber = "88/", SerialNumberSearchType = StringSearchType.Contains };
            var containsResults = await _repository.AdvancedSearchAsync(containsParameters, 1, 25, new());

            SearchParameters equalsParameters = new() { SerialNumber = "9988/782", SerialNumberSearchType = StringSearchType.Equals };
            var equalsResults = await _repository.AdvancedSearchAsync(equalsParameters, 1, 25, new());

            // Test to make sure all three search-type settings is case-insensitive
            SearchParameters startsWithCaseInsensitivityParameters = new() { SerialNumber = "sN:664-4337A1Y", SerialNumberSearchType = StringSearchType.StartsWith };
            var startsWithCaseInsensitivity = await _repository.AdvancedSearchAsync(startsWithCaseInsensitivityParameters, 1, 25, new());

            SearchParameters containsCaseInsensitivityParameters = new() { SerialNumber = "sN:664-4337A1Y", SerialNumberSearchType = StringSearchType.Contains };
            var containsCaseInsensitivity = await _repository.AdvancedSearchAsync(containsCaseInsensitivityParameters, 1, 25, new());

            SearchParameters equalsCaseInsensitivityParameters = new() { SerialNumber = "sN:664-4337A1Y", SerialNumberSearchType = StringSearchType.Equals };
            var equalsCaseInsensitivity = await _repository.AdvancedSearchAsync(equalsCaseInsensitivityParameters, 1, 25, new());

            /*
             * Assert
             */

            Assert.Equal(2, startWithResults.Items.Count);
            Assert.Equal(3, containsResults.Items.Count);
            Assert.Equal(1, equalsResults.Items.Count);

            Assert.Equal(1, startsWithCaseInsensitivity.Items.Count);
            Assert.Equal(1, containsCaseInsensitivity.Items.Count);
            Assert.Equal(1, equalsCaseInsensitivity.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Alias filter")]
        public async Task AdvancedSearch_AliasFilter_AsyncTest()
        {
            /*
             * Arrange
             */



            /*
             * Act
             */

            SearchParameters startWithParameters = new() { Alias = "alia", AliasSearchType = StringSearchType.StartsWith };
            var startWithResults = await _repository.AdvancedSearchAsync(startWithParameters, 1, 25, new());

            SearchParameters containsParameters = new() { Alias = "ias_", AliasSearchType = StringSearchType.Contains };
            var containsResults = await _repository.AdvancedSearchAsync(containsParameters, 1, 25, new());

            SearchParameters equalsParameters = new() { Alias = "alias_1", AliasSearchType = StringSearchType.Equals };
            var equalsResults = await _repository.AdvancedSearchAsync(equalsParameters, 1, 25, new());

            // Test to make sure all three search-type settings is case-insensitive
            SearchParameters startsWithCaseInsensitivityParameters = new() { Alias = "alIAS_1", AliasSearchType = StringSearchType.StartsWith };
            var startsWithCaseInsensitivity = await _repository.AdvancedSearchAsync(startsWithCaseInsensitivityParameters, 1, 25, new());

            SearchParameters containsCaseInsensitivityParameters = new() { Alias = "alIAS_1", AliasSearchType = StringSearchType.Contains };
            var containsCaseInsensitivity = await _repository.AdvancedSearchAsync(containsCaseInsensitivityParameters, 1, 25, new());

            SearchParameters equalsCaseInsensitivityParameters = new() { Alias = "alIAS_1", AliasSearchType = StringSearchType.Equals };
            var equalsCaseInsensitivity = await _repository.AdvancedSearchAsync(equalsCaseInsensitivityParameters, 1, 25, new());

            /*
             * Assert
             */

            Assert.Equal(5, startWithResults.Items.Count);
            Assert.Equal(5, containsResults.Items.Count);
            Assert.Equal(1, equalsResults.Items.Count);

            Assert.Equal(1, startsWithCaseInsensitivity.Items.Count);
            Assert.Equal(1, containsCaseInsensitivity.Items.Count);
            Assert.Equal(1, equalsCaseInsensitivity.Items.Count);
        }

        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Brand filter")]
        public async Task AdvancedSearch_BrandFilter_AsyncTest()
        {
            /*
             * Arrange
             */



            /*
             * Act
             */

            SearchParameters startWithParameters = new() { Brand = "appl", BrandSearchType = StringSearchType.StartsWith };
            var startWithResults = await _repository.AdvancedSearchAsync(startWithParameters, 1, 25, new());

            SearchParameters containsParameters = new() { Brand = "sung", BrandSearchType = StringSearchType.Contains };
            var containsResults = await _repository.AdvancedSearchAsync(containsParameters, 1, 25, new());

            SearchParameters equalsParameters = new() { Brand = "Lenovo", BrandSearchType = StringSearchType.Equals };
            var equalsResults = await _repository.AdvancedSearchAsync(equalsParameters, 1, 25, new());

            // Test to make sure all three search-type settings is case-insensitive
            SearchParameters startsWithCaseInsensitivityParameters = new() { Brand = "sAmSunG", BrandSearchType = StringSearchType.StartsWith };
            var startsWithCaseInsensitivity = await _repository.AdvancedSearchAsync(startsWithCaseInsensitivityParameters, 1, 25, new());

            SearchParameters containsCaseInsensitivityParameters = new() { Brand = "sAmSunG", BrandSearchType = StringSearchType.Contains };
            var containsCaseInsensitivity = await _repository.AdvancedSearchAsync(containsCaseInsensitivityParameters, 1, 25, new());

            SearchParameters equalsCaseInsensitivityParameters = new() { Brand = "sAmSunG", BrandSearchType = StringSearchType.Equals };
            var equalsCaseInsensitivity = await _repository.AdvancedSearchAsync(equalsCaseInsensitivityParameters, 1, 25, new());

            /*
             * Assert
             */

            Assert.Equal(1, startWithResults.Items.Count);
            Assert.Equal(3, containsResults.Items.Count);
            Assert.Equal(2, equalsResults.Items.Count);

            Assert.Equal(3, startsWithCaseInsensitivity.Items.Count);
            Assert.Equal(3, containsCaseInsensitivity.Items.Count);
            Assert.Equal(3, equalsCaseInsensitivity.Items.Count);
        }

        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Product name filter")]
        public async Task AdvancedSearch_ProductNameFilter_AsyncTest()
        {
            /*
             * Arrange
             */



            /*
             * Act
             */

            SearchParameters startWithParameters = new() { ProductName = "Galaxy", ProductNameSearchType = StringSearchType.StartsWith };
            var startWithResults = await _repository.AdvancedSearchAsync(startWithParameters, 1, 25, new());

            SearchParameters containsParameters = new() { ProductName = "s21", ProductNameSearchType = StringSearchType.Contains };
            var containsResults = await _repository.AdvancedSearchAsync(containsParameters, 1, 25, new());

            SearchParameters equalsParameters = new() { ProductName = "Galaxy S21 Ultra", ProductNameSearchType = StringSearchType.Equals };
            var equalsResults = await _repository.AdvancedSearchAsync(equalsParameters, 1, 25, new());

            // Test to make sure all three search-type settings is case-insensitive
            SearchParameters startsWithCaseInsensitivityParameters = new() { ProductName = "GaLaXy S21 Ultra", ProductNameSearchType = StringSearchType.StartsWith };
            var startsWithCaseInsensitivity = await _repository.AdvancedSearchAsync(startsWithCaseInsensitivityParameters, 1, 25, new());

            SearchParameters containsCaseInsensitivityParameters = new() { ProductName = "GaLaXy S21 Ultra", ProductNameSearchType = StringSearchType.Contains };
            var containsCaseInsensitivity = await _repository.AdvancedSearchAsync(containsCaseInsensitivityParameters, 1, 25, new());

            SearchParameters equalsCaseInsensitivityParameters = new() { ProductName = "GaLaXy S21 Ultra", ProductNameSearchType = StringSearchType.Equals };
            var equalsCaseInsensitivity = await _repository.AdvancedSearchAsync(equalsCaseInsensitivityParameters, 1, 25, new());

            /*
             * Assert
             */

            Assert.Equal(3, startWithResults.Items.Count);
            Assert.Equal(2, containsResults.Items.Count);
            Assert.Equal(1, equalsResults.Items.Count);

            Assert.Equal(1, startsWithCaseInsensitivity.Items.Count);
            Assert.Equal(1, containsCaseInsensitivity.Items.Count);
            Assert.Equal(1, equalsCaseInsensitivity.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Start-period filter")]
        public async Task AdvancedSearch_StartPeriodFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters excactDateParameter = new() { StartPeriod = DateTime.Parse("2022-01-15"), StartPeriodSearchType = DateSearchType.ExcactDate };
            var exactDateResults = await _repository.AdvancedSearchAsync(excactDateParameter, 1, 25, new());

            SearchParameters onOrBeforeDateParameters = new() { StartPeriod = DateTime.Parse("2022-01-15"), StartPeriodSearchType = DateSearchType.OnOrBeforeDate };
            var onOrBeforeDateResults = await _repository.AdvancedSearchAsync(onOrBeforeDateParameters, 1, 25, new());

            SearchParameters onOrAfterDateParameters = new() { StartPeriod = DateTime.Parse("2022-01-15"), StartPeriodSearchType = DateSearchType.OnOrAfterDate };
            var onOrAfterDateResults = await _repository.AdvancedSearchAsync(onOrAfterDateParameters, 1, 25, new());


            // Assert
            Assert.Equal(1, exactDateResults.Items.Count);
            Assert.Equal(4, onOrBeforeDateResults.Items.Count);
            Assert.Equal(3, onOrAfterDateResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: End-period filter")]
        public async Task AdvancedSearch_EndPeriodFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters excactDateParameter = new() { EndPeriod = DateTime.Parse("2023-01-15"), EndPeriodSearchType = DateSearchType.ExcactDate };
            var exactDateResults = await _repository.AdvancedSearchAsync(excactDateParameter, 1, 25, new());

            SearchParameters onOrBeforeDateParameters = new() { EndPeriod = DateTime.Parse("2023-01-15"), EndPeriodSearchType = DateSearchType.OnOrBeforeDate };
            var onOrBeforeDateResults = await _repository.AdvancedSearchAsync(onOrBeforeDateParameters, 1, 25, new());

            SearchParameters onOrAfterDateParameters = new() { EndPeriod = DateTime.Parse("2023-01-15"), EndPeriodSearchType = DateSearchType.OnOrAfterDate };
            var onOrAfterDateResults = await _repository.AdvancedSearchAsync(onOrAfterDateParameters, 1, 25, new());


            // Assert
            Assert.Equal(1, exactDateResults.Items.Count);
            Assert.Equal(4, onOrBeforeDateResults.Items.Count);
            Assert.Equal(3, onOrAfterDateResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Purchase date filter")]
        public async Task AdvancedSearch_PurchaseDateFilter_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters excactDateParameter = new() { PurchaseDate = DateTime.Parse("2022-01-15"), PurchaseDateSearchType = DateSearchType.ExcactDate };
            var exactDateResults = await _repository.AdvancedSearchAsync(excactDateParameter, 1, 25, new());

            SearchParameters onOrBeforeDateParameters = new() { PurchaseDate = DateTime.Parse("2022-01-15"), PurchaseDateSearchType = DateSearchType.OnOrBeforeDate };
            var onOrBeforeDateResults = await _repository.AdvancedSearchAsync(onOrBeforeDateParameters, 1, 25, new());

            SearchParameters onOrAfterDateParameters = new() { PurchaseDate = DateTime.Parse("2022-01-15"), PurchaseDateSearchType = DateSearchType.OnOrAfterDate };
            var onOrAfterDateResults = await _repository.AdvancedSearchAsync(onOrAfterDateParameters, 1, 25, new());


            // Assert
            Assert.Equal(1, exactDateResults.Items.Count);
            Assert.Equal(4, onOrBeforeDateResults.Items.Count);
            Assert.Equal(3, onOrAfterDateResults.Items.Count);
        }


        /// <summary> Test for <see cref="AssetLifecycleRepository.AdvancedSearchAsync(SearchParameters, int, int, System.Threading.CancellationToken)"/> </summary>
        [Fact(DisplayName = "Advanced Search: Quick search")]
        public async Task AdvancedSearch_QuickSearch_AsyncTest()
        {
            // Arrange


            // Act
            SearchParameters phoneImeiEquals = new() { QuickSearch = "334937775025511", QuickSearchSearchType = StringSearchType.Equals };
            var phoneImeiEqualsResults = await _repository.AdvancedSearchAsync(phoneImeiEquals, 1, 25, new());

            SearchParameters phoneImeiStartsWith = new() { QuickSearch = "3349", QuickSearchSearchType = StringSearchType.StartsWith };
            var phoneImeiStartsWithResults = await _repository.AdvancedSearchAsync(phoneImeiStartsWith, 1, 25, new());

            SearchParameters phoneImeiContains = new() { QuickSearch = "77750", QuickSearchSearchType = StringSearchType.Contains };
            var phoneImeiContainsResults = await _repository.AdvancedSearchAsync(phoneImeiContains, 1, 25, new());

            SearchParameters tabletImeiEquals = new() { QuickSearch = "869302915930937", QuickSearchSearchType = StringSearchType.Equals };
            var tabletImeiEqualsResults = await _repository.AdvancedSearchAsync(tabletImeiEquals, 1, 25, new());

            SearchParameters tabletImeiStartsWith = new() { QuickSearch = "8693", QuickSearchSearchType = StringSearchType.StartsWith };
            var tabletImeiStartsWithResults = await _repository.AdvancedSearchAsync(tabletImeiStartsWith, 1, 25, new());

            SearchParameters tabletImeiContains = new() { QuickSearch = "029159309", QuickSearchSearchType = StringSearchType.Contains };
            var tabletImeiContainsResults = await _repository.AdvancedSearchAsync(tabletImeiContains, 1, 25, new());


            SearchParameters phoneSerialEquals = new() { QuickSearch = "LL6-dd5DaYY6", QuickSearchSearchType = StringSearchType.Equals };
            var phoneSerialEqualsResults = await _repository.AdvancedSearchAsync(phoneSerialEquals, 1, 25, new());

            SearchParameters phoneSerialStartsWith = new() { QuickSearch = "LL6-dd", QuickSearchSearchType = StringSearchType.StartsWith };
            var phoneSerialStartsWithResults = await _repository.AdvancedSearchAsync(phoneSerialStartsWith, 1, 25, new());

            SearchParameters phoneSerialContains = new() { QuickSearch = "d5DaYY6", QuickSearchSearchType = StringSearchType.Contains };
            var phoneSerialContainsResults = await _repository.AdvancedSearchAsync(phoneSerialContains, 1, 25, new());

            SearchParameters tabletSerialEquals = new() { QuickSearch = "SN:664-9977A1Y", QuickSearchSearchType = StringSearchType.Equals };
            var tabletSerialEqualsResults = await _repository.AdvancedSearchAsync(tabletSerialEquals, 1, 25, new());

            SearchParameters tabletSerialStartsWith = new() { QuickSearch = "SN:664-9977", QuickSearchSearchType = StringSearchType.StartsWith };
            var tabletSerialStartsWithResults = await _repository.AdvancedSearchAsync(tabletSerialStartsWith, 1, 25, new());

            SearchParameters tabletSerialContains = new() { QuickSearch = "9977A1Y", QuickSearchSearchType = StringSearchType.Contains };
            var tabletSerialContainsResults = await _repository.AdvancedSearchAsync(tabletSerialContains, 1, 25, new());



            SearchParameters contractHolderNameEquals = new() { QuickSearch = "John Doe", QuickSearchSearchType = StringSearchType.Equals };
            var contractHolderNameEqualsResults = await _repository.AdvancedSearchAsync(contractHolderNameEquals, 1, 25, new());

            SearchParameters contractHolderNameStartsWith = new() { QuickSearch = "John", QuickSearchSearchType = StringSearchType.StartsWith };
            var contractHolderNameStartsWithResults = await _repository.AdvancedSearchAsync(contractHolderNameStartsWith, 1, 25, new());

            SearchParameters contractHolderNameContains = new() { QuickSearch = "n do", QuickSearchSearchType = StringSearchType.Contains };
            var contractHolderNameContainsResults = await _repository.AdvancedSearchAsync(contractHolderNameContains, 1, 25, new());


            // Assert
            Assert.Equal(1, phoneImeiEqualsResults.Items.Count);
            Assert.Equal(1, phoneImeiStartsWithResults.Items.Count);
            Assert.Equal(1, phoneImeiContainsResults.Items.Count);

            Assert.Equal(1, tabletImeiEqualsResults.Items.Count);
            Assert.Equal(1, tabletImeiStartsWithResults.Items.Count);
            Assert.Equal(1, tabletImeiContainsResults.Items.Count);

            Assert.Equal(1, phoneSerialEqualsResults.Items.Count);
            Assert.Equal(1, phoneSerialStartsWithResults.Items.Count);
            Assert.Equal(1, phoneSerialContainsResults.Items.Count);

            Assert.Equal(1, tabletSerialEqualsResults.Items.Count);
            Assert.Equal(1, tabletSerialStartsWithResults.Items.Count);
            Assert.Equal(1, tabletSerialContainsResults.Items.Count);

            Assert.Equal(1, contractHolderNameEqualsResults.Items.Count);
            Assert.Equal(1, contractHolderNameStartsWithResults.Items.Count);
            Assert.Equal(1, contractHolderNameContainsResults.Items.Count);
        }
    }
}