using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal class FeatureTypeRepository : TranslationRepository<ProductCatalogContext, FeatureType, FeatureTypeTranslation>, IFeatureTypeRepository
    {
        public FeatureTypeRepository(ref ProductCatalogContext context) : base(ref context)
        {
        }
    }
}
