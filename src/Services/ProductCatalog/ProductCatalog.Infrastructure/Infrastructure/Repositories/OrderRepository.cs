using Boilerplate.EntityFramework.Generics.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal class OrderRepository : TemporalReadWriteRepository<ProductCatalogContext, Order>, IOrderRepository
    {
        public OrderRepository(ref ProductCatalogContext context) : base(ref context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(bool asNoTracking, Expression<Func<Order, bool>>? filter = null)
        {
            IQueryable<Order> query = _context.Orders;

            if (filter is not null)
                query = query.Where(filter);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.Select(e => e.ProductId)
                              .Distinct()
                              .ToListAsync();
        }

        // TODO: Add includes for the requirements
        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetProductsFromOrdersAsync(bool includeTranslations, bool asNoTracking, Expression<Func<Order, bool>>? filter = null)
        {
            IQueryable<Order> query = _context.Orders;

            if (filter is not null)
                query = query.Where(filter);

            if (includeTranslations)
                query = query.Include(e => e.Product!.Translations);

            return await query.Select(e => e.Product!)
                              .Distinct()
                              .ToListAsync();
        }
    }
}
