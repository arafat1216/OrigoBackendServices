using ProductCatalog.Service.Models.Database;
using ProductCatalog.Service.Infrastructure.Spesification;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> FindWithSpecificationPattern(ISpecification<Product>? specification = null);
    }
}
