using Boilerplate.EntityFramework.Generics.UnitOfWork;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure.Repositories;

namespace ProductCatalog.Infrastructure.Infrastructure
{
    internal class UnitOfWork : UnitOfWorkRoot<ProductCatalogContext>, IUnitOfWork
    {
        public IFeatureRepository Features { get; private set; }
        public IFeatureTypeRepository FeaturesTypes { get; private set; }
        public IProductRepository Products { get; private set; }
        public IProductTypeRepository ProductTypes { get; private set; }
        public IOrderRepository Orders { get; private set; }

        public UnitOfWork(ProductCatalogContext context) : base(ref context)
        {
            Features = new FeatureRepository(ref context);
            FeaturesTypes = new FeatureTypeRepository(ref context);
            Products = new ProductRepository(ref context);
            ProductTypes = new ProductTypeRepository(ref context);
            Orders = new OrderRepository(ref context);
        }
    }
}
