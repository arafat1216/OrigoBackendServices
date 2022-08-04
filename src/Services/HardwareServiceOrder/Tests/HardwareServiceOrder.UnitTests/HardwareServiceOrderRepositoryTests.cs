using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderRepositoryTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly HardwareServiceOrderContext _dbContext;
        private readonly IHardwareServiceOrderRepository _repository;
        public HardwareServiceOrderRepositoryTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>()

        .UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsrepository.db").Options)
        {
            _dbContext = new HardwareServiceOrderContext(ContextOptions);
            _repository = new HardwareServiceOrderRepository(_dbContext);
        }

        [Fact]
        public async Task GetAllOrders()
        {
            var orders = await _repository.GetAllOrdersAsync();
            Assert.Equal(4, orders.Count());
        }

        [Fact]
        public async Task GetAllOrdersOlderThan()
        {
            var orders = await _repository.GetAllOrdersAsync(DateTime.Today.AddDays(-7));
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task GetAllOrdersByStatuses()
        {
            var orders = await _repository.GetAllOrdersAsync(DateTime.Today.AddDays(-7), statusIds: new List<int> { 200 });
            Assert.Single(orders);

            orders = await _repository.GetAllOrdersAsync(statusIds: new List<int> { 200, 300 });
            Assert.Equal(2, orders.Count());
        }
    }
}
