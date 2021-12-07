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
        /// <exception cref="ArgumentNullException"> Thrown when all parameters are <see langword="null"/>. </exception>
        public async Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null)
        {
            if (filter is null)
            {
                return await _context.Orders
                                     .Select(e => e.ProductId)
                                     .Distinct()
                                     .ToListAsync();
            }
            else
            {
                return await _context.Orders
                                     .Where(filter)
                                     .Select(e => e.ProductId)
                                     .Distinct()
                                     .ToListAsync();
            }

        }

        // TODO: Add includes for the requirements
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"> Thrown when all parameters are <see langword="null"/>. </exception>
        public async Task<IEnumerable<Product>> GetProductsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null)
        {
            if (filter is null)
            {
                return await _context.Orders
                                     .Include(e => e.Product!.Translations)
                                     .Select(e => e.Product!)
                                     .Distinct()
                                     .ToListAsync();
            }
            else
            {
                return await _context.Orders
                                     .Where(filter)
                                     .Include(e => e.Product!.Translations)
                                     .Select(e => e.Product!)
                                     .Distinct()
                                     .ToListAsync();
            }

        }
    }
}
