using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IProductRepository : ITranslationRepository<Product>
    {
    }
}
