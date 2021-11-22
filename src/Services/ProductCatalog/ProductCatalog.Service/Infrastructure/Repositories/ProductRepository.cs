using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Infrastructure.Spesification;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class ProductRepository : Repository<Product, ProductCatalogContext>, IProductRepository
    {
        public ProductRepository(ProductCatalogContext context) : base(context)
        {
        }

        public IEnumerable<Product> FindWithSpecificationPattern(ISpecification<Product>? specification = null)
        {
            throw new NotImplementedException();
        }
    }
}
