using Xunit;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrder.UnitTests;
using Microsoft.EntityFrameworkCore;
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


        public HardwareServiceOrderRepositoryTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>().UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsrepository.db").Options)
        {
            _dbContext = new HardwareServiceOrderContext(ContextOptions);
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
    }
}