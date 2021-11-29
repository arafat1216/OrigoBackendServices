using ProductCatalog.Service.Models.Database;
using ProductCatalog.Service.Infrastructure.Spesification;
using ProductCatalog.Service.Infrastructure.Repositories.Boilerplate;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IProductRepository : ITranslationRepository<Product>
    {
        IEnumerable<Product> FindWithSpecificationPattern(ISpecification<Product>? specification = null);
    }
}
