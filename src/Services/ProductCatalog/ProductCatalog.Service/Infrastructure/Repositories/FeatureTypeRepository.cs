using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class FeatureTypeRepository : TranslationRepository<FeatureType, FeatureTypeTranslation, ProductCatalogContext>, IFeatureTypeRepository
    {
        public FeatureTypeRepository(ProductCatalogContext context) : base(context)
        {
        }
    }
}
