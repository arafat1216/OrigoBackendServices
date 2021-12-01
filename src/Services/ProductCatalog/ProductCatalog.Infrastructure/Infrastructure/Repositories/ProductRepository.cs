using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Infrastructure.Spesification;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal class ProductRepository : TranslationRepository<ProductCatalogContext, Product, ProductTranslation>, IProductRepository
    {
        public ProductRepository(ref ProductCatalogContext context) : base(ref context)
        {
        }


    }
}
