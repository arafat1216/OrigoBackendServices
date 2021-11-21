using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Spesification;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class ProductRepository<TDbContext> : Repository<Product, TDbContext>, IProductRepository where TDbContext : DbContext
    {
        public ProductRepository(TDbContext context) : base(context)
        {
        }

        public IEnumerable<Product> FindWithSpecificationPattern(ISpecification<Product>? specification = null)
        {
            throw new NotImplementedException();
        }
    }
}
