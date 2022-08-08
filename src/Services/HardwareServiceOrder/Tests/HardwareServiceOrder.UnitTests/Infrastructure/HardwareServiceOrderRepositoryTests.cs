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


        public HardwareServiceOrderRepositoryTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>().UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsrepository.db").Options)
        {
            _dbContext = new HardwareServiceOrderContext(ContextOptions);
            _repository = new HardwareServiceOrderRepository(_dbContext);
        }


        /// <summary>
        ///     See if we were able to retrieve a service provider, and that it's requested properties were correctly included.
        /// </summary>
        /// <remarks>
        ///     We are doing all testing against 'ID = 1' that is added in the EF seeding data.
        /// </remarks>
        /// <returns></returns>
        [Fact()]
        public async Task GetAllServiceProviders()
        {
            var allSericeProviders = await _repository.GetAllServiceProvidersAsync(true, true, true);
            ServiceProvider? serviceProvider = allSericeProviders.FirstOrDefault(e => e.Id == 1);

            // Assert
            Assert.True(allSericeProviders.Any());                          // Do we have any results?

            Assert.NotNull(serviceProvider);                                // Are the seeded entry present?
            Assert.NotNull(serviceProvider?.OfferedServiceOrderAddons);     // This should never be null if the include worked
            Assert.NotNull(serviceProvider?.SupportedServiceTypes);         // This should never be null if the include worked

            Assert.True(serviceProvider?.OfferedServiceOrderAddons?.Any()); // Are there any items present? We have seeded at least one.
            Assert.True(serviceProvider?.SupportedServiceTypes?.Any());     // Are there any items present? We have seeded at least one.
        }

    }
}