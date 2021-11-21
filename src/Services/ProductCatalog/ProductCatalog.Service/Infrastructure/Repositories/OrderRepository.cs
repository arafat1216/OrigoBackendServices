using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class OrderRepository<TDbContext> : Repository<Order, TDbContext>, IOrderRepository where TDbContext : DbContext
    {
        public OrderRepository(TDbContext context) : base(context)
        {
        }

    }
}
