using Boilerplate.EntityFramework.Generics.Repositories;
using ProductCatalog.Infrastructure.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IOrderRepository : ITemporalReadWriteRepository<Order>
    {
        Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null);

        Task<IEnumerable<Product>> GetProductsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null);
    }
}
