using Xunit;
using HardwareServiceOrderServices.Infrastructure;
using Common.Seedwork;
using HardwareServiceOrder.UnitTests;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using System.Threading.Tasks;

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
            _repository = new HardwareServiceOrderRepository(_dbContext);
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
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            // Act
            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "username", "password");
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
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "OldUsername", "OldPassword");
            ApiCredential apiCredential2 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "NewUsername", "NewPassword");

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
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, null, "OldUsername", "OldPassword");
            ApiCredential apiCredential2 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, null, "NewUsername", "NewPassword");

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
            CustomerServiceProvider customerServiceProvider = new(Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, null);
            await _dbContext.AddAsync(customerServiceProvider);
            await _dbContext.SaveChangesAsync();

            ApiCredential apiCredential1 = await _repository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, (int)ServiceTypeEnum.SUR, "OldUsername", "OldPassword");

            // Act
            _repository.Delete(apiCredential1).Wait();
            ApiCredential? result = await _dbContext.ApiCredentials.FindAsync(apiCredential1.Id);

            // Assert
            Assert.Null(result);
        }
    }
}