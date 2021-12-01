using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal class ProductTypeRepository : TranslationRepository<ProductCatalogContext, ProductType, ProductTypeTranslation>, IProductTypeRepository
    {
        public ProductTypeRepository(ref ProductCatalogContext context) : base(ref context)
        {
        }


    }
}
