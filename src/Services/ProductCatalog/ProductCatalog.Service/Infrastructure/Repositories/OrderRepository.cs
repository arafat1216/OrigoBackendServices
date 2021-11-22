using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class OrderRepository : Repository<Order, ProductCatalogContext>, IOrderRepository
    {
        public OrderRepository(ProductCatalogContext context) : base(context)
        {
        }


        public async Task<IEnumerable<int>> GetProductIdsByOrganization(Guid organizationId)
        {
            return await _context.Orders
                                 .Where(e => e.OrganizationId == organizationId)
                                 .Select(e => e.ProductId)
                                 .Distinct()
                                 .ToListAsync();
        }
    }
}
