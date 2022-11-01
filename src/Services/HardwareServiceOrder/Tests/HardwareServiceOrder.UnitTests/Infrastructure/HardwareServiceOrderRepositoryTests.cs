using Xunit;
using HardwareServiceOrderServices.Infrastructure;
using Common.Seedwork;
using HardwareServiceOrder.UnitTests;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Google.Api;

namespace HardwareServiceOrderServices.Infrastructure.Tests
{
    public class HardwareServiceOrderRepositoryTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly HardwareServiceOrderContext _dbContext;
        private readonly IHardwareServiceOrderRepository _repository;

        /// <summary>
        ///     The ID for the service-provider test-data that is added in <see cref="HardwareServiceOrderServiceBaseTests.Seed"/>.
        /// </summary>
        private readonly int _testServiceProviderId = 700;
        private static readonly Guid callerId = Guid.Parse("9cd6fb1f-67f0-45bf-b0ba-1c00e78a1fb8");

        public HardwareServiceOrderRepositoryTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>().UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsrepository.db").Options)
        {
            // Mock the interceptor that stores the caller-ID for the incoming ASP.NET HTTP request.
            // This is needed if we want to add the EF save-interceptor (auto assignment of the auditing attributes).
            var mock = new Mock<IApiRequesterService>();
            mock.Setup(e => e.AuthenticatedUserId).Returns(callerId);

            _dbContext = new HardwareServiceOrderContext(ContextOptions, mock.Object);
            _repository = new HardwareServiceOrderRepository(_dbContext, new EphemeralDataProtectionProvider());
        }


        /// <summary>
        ///     See if we were able to retrieve a service provider, and that it's requested properties were correctly included.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Get all service providers")]
        public async Task GetAllServiceProviders()
        {
            var allSericeProviders = await _repository.GetAllServiceProvidersAsync(true, true, true);
            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == _testServiceProviderId);

            // Assert
            Assert.True(allSericeProviders.Any());                          // Do we have any results?

            Assert.NotNull(serviceProvider);                                // Are the seeded entry present?
            Assert.NotNull(serviceProvider?.OfferedServiceOrderAddons);     // This should never be null if the include worked
            Assert.NotNull(serviceProvider?.SupportedServiceTypes);         // This should never be null if the include worked

            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Any()); // Are there any items present? We have seeded at least one.
            Assert.True(serviceProvider?.SupportedServiceTypes?.Any());     // Are there any items present? We have seeded at least one.
        }


        /// <summary>
        ///     Makes sure we are still able to retrieve the expected results when applying a conditional include-filter to the service-provider list.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Get all service-providers w/addon filter: No filtering")]
        public async Task GetAllServiceProvidersWithAddonFilter_NoFilter_AsyncTest()

        {


            var allSericeProviders = await _repository.GetAllServiceProvidersWithAddonFilterAsync(false,
                                                                                                  false,
                                                                                                  true,
                                                                                                  true);

            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == _testServiceProviderId);

            // Assert

            // Do we have any results?
            Assert.True(allSericeProviders.Any(), "The service-provider list is empty, but was expected to contain test-data.");

            // Are the seeded entry present?
            Assert.NotNull(serviceProvider);

            // This should never be null if the include worked
            Assert.NotNull(serviceProvider?.OfferedServiceOrderAddons);

            // This should never be null if the include worked
            Assert.NotNull(serviceProvider?.SupportedServiceTypes);

            // Are there any items present? We have seeded at least one.
            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Count == 4, $"We expected 4 service-addons, but received {serviceProvider?.OfferedServiceOrderAddons?.Count}. Has the seeding-data changed?");

            // Are there any items present? We have seeded at least one.
            Assert.True(serviceProvider?.SupportedServiceTypes?.Any(), "The supported service-types list is empty, but was expected to contain test-data.");
        }


        /// <summary>
        ///     Makes sure we are still able to retrieve the expected results when applying a conditional include-filter to the service-provider list.
        /// </summary>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Get all service-providers w/addon filter: Only customer togglable")]
        public async Task GetAllServiceProvidersWithAddonFilter_OnlyCustomerTogglable_AsyncTest()
        {
            var allSericeProviders = await _repository.GetAllServiceProvidersWithAddonFilterAsync(true,
                                                                                                  false,
                                                                                                  false,
                                                                                                  true);

            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == _testServiceProviderId);

            // Assert

            // The test-data item should only have two entries matching these conditions
            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Count == 2, $"We expected 2 service-addons, but received {serviceProvider?.OfferedServiceOrderAddons?.Count}. Has the seeding-data changed?");
        }


        /// <summary>
        ///     Makes sure we are still able to retrieve the expected results when applying a conditional include-filter to the service-provider list.
        /// </summary>
        /// <remarks>
        ///     This extends <see cref="GetAllServiceProvidersWithAddonFilter_NoFilter_AsyncTest"/>, and explicitly tests the filtering.
        ///     This means the primary test must pass before this one will.
        /// </remarks>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Get all service-providers w/addon filter: Only user selectable")]
        public async Task GetAllServiceProvidersWithAddonFilter_OnlyUserSelectable_AsyncTest()
        {
            var allSericeProviders = await _repository.GetAllServiceProvidersWithAddonFilterAsync(false,
                                                                                                  true,
                                                                                                  false,
                                                                                                  true);

            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == _testServiceProviderId);

            // Assert

            // The test-data item should only have two entries matching these conditions
            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Count == 2, $"We expected 2 service-addons, but received {serviceProvider?.OfferedServiceOrderAddons?.Count}. Has the seeding-data changed?");
        }

        /// <summary>
        ///     Makes sure we are still able to retrieve the expected results when applying a conditional include-filter to the service-provider list.
        /// </summary>
        /// <remarks>
        ///     This extends <see cref="GetAllServiceProvidersWithAddonFilter_NoFilter_AsyncTest"/>, and explicitly tests the filtering.
        ///     This means the primary test must pass before this one will.
        /// </remarks>
        /// <returns> The awaitable task. </returns>
        [Fact(DisplayName = "Get all service-providers w/addon filter: All filters")]
        public async Task GetAllServiceProvidersWithAddonFilter_AllFilters_AsyncTest()
        {
            var allSericeProviders = await _repository.GetAllServiceProvidersWithAddonFilterAsync(true,
                                                                                                  true,
                                                                                                  false,
                                                                                                  true);

            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == _testServiceProviderId);

            // Assert

            // The test-data item should only have one entry matching these conditions
            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Count == 1, $"We expected 1 service-addons, but received {serviceProvider?.OfferedServiceOrderAddons?.Count}. Has the seeding-data changed?");
        }

        [Fact()]
        public async Task AddOrUpdateApiCredential_AddNewCredential_Test()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            // Act
            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "username", "password");
            var result = await _dbContext.ApiCredentials.FindAsync(apiCredential1.Id);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(customerServiceProvider.Id, result?.CustomerServiceProviderId);
            Assert.Equal(apiCredential1.ServiceTypeId, result?.ServiceTypeId);
            Assert.Equal(apiCredential1.ApiUsername, result?.ApiUsername);
            Assert.Equal(apiCredential1.ApiPassword, result?.ApiPassword);
            Assert.Equal(apiCredential1.LastUpdateFetched, result?.LastUpdateFetched);
        }


        [Fact()]
        public async Task AddOrUpdateApiCredential_UpdateExistingCredential_Test()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "OldUsername", "OldPassword");
            ApiCredential apiCredential2 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "NewUsername", "NewPassword");

            // Act
            var result = await _dbContext.ApiCredentials.FindAsync(apiCredential2.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerServiceProvider.Id, result?.CustomerServiceProviderId);
            Assert.Equal(apiCredential2.ServiceTypeId, result?.ServiceTypeId);
            Assert.Equal(apiCredential2.ApiUsername, result?.ApiUsername);
            Assert.Equal(apiCredential2.ApiPassword, result?.ApiPassword);
            Assert.Equal(apiCredential2.LastUpdateFetched, result?.LastUpdateFetched);
        }


        // Ensure the alternate-key / unique index still works (no duplicate null entries)
        [Fact()]
        public async Task AddOrUpdateApiCredential_UpdateExistingCredential_NullUnique_Test()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, null, "OldUsername", "OldPassword");
            ApiCredential apiCredential2 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, null, "NewUsername", "NewPassword");

            // Act
            var result = await _dbContext.ApiCredentials
                                         .Where(e => e.CustomerServiceProviderId == customerServiceProvider.Id && e.ServiceTypeId == null)
                                         .ToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1); // We should not have duplicates. The new entry should be come a update for the existing one!
        }


        [Fact()]
        public async Task DeleteApiCredentialAsyncTest()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(CUSTOMER_ONE_ID, customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "OldUsername", "OldPassword");

            // Act
            _repository.DeleteAndSaveAsync(apiCredential1).Wait();
            ApiCredential? result = await _dbContext.ApiCredentials.FindAsync(apiCredential1.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact()]
        public async Task CreateHardwareServiceOrderAsyncTest()
        {
            // Arrange
            AssetInfo assetInfo = new("Brand", "Model", new HashSet<string>() { "IMEI1", "IMEI2" }, "S/N", DateOnly.Parse("01-01-2020"), new List<string>() { "Charger" });
            ContactDetails owner = new(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", "+4799988777");
            DeliveryAddress deliveryAddress = new(RecipientTypeEnum.Personal, "John Doe", "MyStreet", "C/O: Jane Doe", "1234", "City", "NO");
            List<ServiceEvent> serviceEvents = new();
            HashSet<int> includedServiceAddonIds = new() { 1 };

            Models.HardwareServiceOrder serviceOrder = new(CUSTOMER_ONE_ID, Guid.NewGuid(), 1, assetInfo, "A general description", owner, deliveryAddress, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, includedServiceAddonIds, "ServiceProviderID1", "ServiceProviderID2", "https://www.example.com", serviceEvents);

            // Act
            var createResult = await _repository.CreateHardwareServiceOrderAsync(serviceOrder);
            var dbResult = await _dbContext.HardwareServiceOrders.FindAsync(createResult.Id);

            // Assert
            Assert.NotNull(createResult);
            Assert.NotNull(dbResult);

            Assert.Equal(serviceOrder.AssetInfo.Imei!.Count, dbResult.AssetInfo.Imei?.Count);
            Assert.Equal(serviceOrder.AssetInfo.PurchaseDate, dbResult.AssetInfo.PurchaseDate);
            Assert.Equal(serviceOrder.IncludedServiceOrderAddonIds!.Count, dbResult.IncludedServiceOrderAddonIds?.Count);
        }


        // We want to test that the JSON serialization/de-serialization that is used on some properties don't throw errors during DB read/write operations.
        [Fact()]
        public async Task CreateHardwareServiceOrder_Test_JsonSerialization_WithNullValues_AsyncTest()
        {
            // Arrange
            AssetInfo assetInfo = new(null, null, null, null, null, null);
            ContactDetails owner = new(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", null);
            List<ServiceEvent> serviceEvents = new();

            Models.HardwareServiceOrder serviceOrder = new(CUSTOMER_ONE_ID, Guid.NewGuid(), 1, assetInfo, "A general description", owner, null, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, null, "ServiceProviderID1", null, null, serviceEvents);

            // Act
            var createResult = await _repository.CreateHardwareServiceOrderAsync(serviceOrder);
            var dbResult = await _dbContext.HardwareServiceOrders.FindAsync(createResult.Id);

            // Assert
            Assert.NotNull(createResult);
            Assert.NotNull(dbResult);
        }

        // Test specifically for the "search" parameter
        [Fact()]
        public async Task GetAllServiceOrdersForOrganizationAsync_Search_Parameter_Test()
        {
            // Arrange
            Guid customerId = Guid.Parse("d45a6943-75ce-402e-a612-04900aa50059");

            DeliveryAddress deliveryAddress1 = new(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");

            AssetInfo assetInfo1 = new("Apple", "iPhone 10", new HashSet<string>() { "914364591085175" }, "SN/88879", DateOnly.Parse("2021-12-29"), null);
            AssetInfo assetInfo2 = new("Samsung", "Galaxy S22 Ultra", new HashSet<string>() { "457046821986701" }, "SN/66647", DateOnly.Parse("2022-05-28"), null);

            var order1 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 1, assetInfo1, "My screen broke!", new ContactDetails(Guid.NewGuid(), "John", "Doe", "john@test.com", "+4799988777"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, null, "10000000-0000-0000-0000-000000000000", "NOLF51234", "externalLink", new List<ServiceEvent>());
            var order2 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 2, assetInfo2, "The battery won't charge", new ContactDetails(Guid.NewGuid(), "Ola", "Normann", "ola@test.com", "+4766688999"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.CompletedRepaired, (int)ServiceProviderEnum.ConmodoNo, null, "20000000-0000-0000-0000-000000000000", "NOLF8852", "externalLink", new List<ServiceEvent>());

            _dbContext.Add(order1);
            _dbContext.Add(order2);
            _dbContext.SaveChanges();

            // Act
            var noFilter = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new());
            var searchResultOrderId1 = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), search: "10000000-");
            var searchResultOrderId2 = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), search: "NOLF8852");
            var searchResultNoResults = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), search: "NOLF0999487");

            // Assert
            Assert.Equal(2, noFilter.Items.Count);
            Assert.Equal(1, searchResultOrderId1.Items.Count);
            Assert.Equal(1, searchResultOrderId2.Items.Count);
            Assert.Equal(0, searchResultNoResults.Items.Count);
        }

        // Test specifically for the "status ID" parameter
        [Fact()]
        public async Task GetAllServiceOrdersForOrganizationAsync_StatusIdFilter_Tests()
        {
            // Arrange
            Guid customerId = Guid.Parse("d45a6943-75ce-402e-a612-04900aa50059");

            DeliveryAddress deliveryAddress1 = new(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");

            AssetInfo assetInfo1 = new("Apple", "iPhone 10", new HashSet<string>() { "914364591085175" }, "SN/88879", DateOnly.Parse("2021-12-29"), null);
            AssetInfo assetInfo2 = new("Samsung", "Galaxy S22 Ultra", new HashSet<string>() { "457046821986701" }, "SN/66647", DateOnly.Parse("2022-05-28"), null);

            var order1 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 1, assetInfo1, "My screen broke!", new ContactDetails(Guid.NewGuid(), "John", "Doe", "john@test.com", "+4799988777"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, null, "10000000-0000-0000-0000-000000000000", "NOLF51234", "externalLink", new List<ServiceEvent>());
            var order2 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 2, assetInfo2, "The battery won't charge", new ContactDetails(Guid.NewGuid(), "Ola", "Normann", "ola@test.com", "+4766688999"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.CompletedRepaired, (int)ServiceProviderEnum.ConmodoNo, null, "20000000-0000-0000-0000-000000000000", "NOLF8852", "externalLink", new List<ServiceEvent>());

            _dbContext.Add(order1);
            _dbContext.Add(order2);
            _dbContext.SaveChanges();

            // Act

            var searchResultsWithUsedAndUnusedIds = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), statusIds: new HashSet<int>() { 3, 6, 11 });
            var searchResultsWithOnlyUnusedIds = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), statusIds: new HashSet<int>() { 3 });
            var searchResultsWithOnlyUsedIds = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), statusIds: new HashSet<int>() { 6 });

            // Assert
            Assert.Equal(2, searchResultsWithUsedAndUnusedIds.Items.Count);
            Assert.Equal(0, searchResultsWithOnlyUnusedIds.Items.Count);
            Assert.Equal(1, searchResultsWithOnlyUsedIds.Items.Count);
        }

        // Test specifically for the search string when searching on the assets attributes
        [Fact()]
        public async Task GetAllServiceOrdersForOrganizationAsync_FilterSearchOnAssetAttributes_ShouldReturnOnlyBasedOnAssetAttributes()
        {
            // Arrange
            Guid customerId = Guid.Parse("d45a6943-75ce-402e-a612-04900aa50059");

            DeliveryAddress deliveryAddress1 = new(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");

            AssetInfo assetInfo1 = new("Samsung", "Galaxy S22 Ultra", new HashSet<string>() { "914364591085175" }, "SN/88879", DateOnly.Parse("2021-12-29"), null);
            AssetInfo assetInfo2 = new("Apple", "iPhone 10", new HashSet<string>() { "457046821986701" }, "SN/66647", DateOnly.Parse("2022-05-28"), null);

            var order1 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 1, assetInfo1, "My screen broke!", new ContactDetails(Guid.NewGuid(), "John", "Doe", "john@test.com", "+4799988777"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, null, "10000000-0000-0000-0000-000000000000", "NOLF51234", "externalLink", new List<ServiceEvent>());
            var order2 = new Models.HardwareServiceOrder(customerId, Guid.NewGuid(), 2, assetInfo2, "The battery won't charge", new ContactDetails(Guid.NewGuid(), "Ola", "Normann", "ola@test.com", "+4766688999"), deliveryAddress1, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.CompletedRepaired, (int)ServiceProviderEnum.ConmodoNo, null, "20000000-0000-0000-0000-000000000000", "NOLF8852", "externalLink", new List<ServiceEvent>());

            _dbContext.Add(order1);
            _dbContext.Add(order2);
            _dbContext.SaveChanges();

            // Act

            var searchResultsWithSearchString = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), null, search: "s");
            var searchResultWithDigits = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(),null,search: "10");
            var searchResultBasedOnModel = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), null, search: "Galaxy S22 Ultra");
            var searchResultBasedOnBrand = await _repository.GetAllServiceOrdersForOrganizationAsync(customerId, null, null, false, 1, 10, true, new(), null, search: "Apple");

            // Assert
            Assert.Equal(1, searchResultsWithSearchString.Items.Count);
            Assert.Equal(2, searchResultWithDigits.Items.Count);
            Assert.Equal(1, searchResultBasedOnModel.Items.Count);
            Assert.Equal(1, searchResultBasedOnBrand.Items.Count);
        }
    }
}