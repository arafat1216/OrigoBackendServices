using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class ProductTypeRepository : TranslationRepository<ProductType, ProductTypeTranslation, ProductCatalogContext>, IProductTypeRepository
    {
        public ProductTypeRepository(ProductCatalogContext context) : base(context)
        {
        }


    }
}
