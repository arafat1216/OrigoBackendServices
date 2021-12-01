using ProductCatalog.Infrastructure.Models.Database;
using ProductCatalog.Infrastructure.Infrastructure.Spesification;
using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IProductRepository : ITranslationRepository<Product>
    {
    }
}
