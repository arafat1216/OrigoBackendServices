using Boilerplate.EntityFramework.Generics.UnitOfWork;
using ProductCatalog.Infrastructure.Infrastructure.Repositories;

namespace ProductCatalog.Infrastructure.Infrastructure
{
    internal interface IUnitOfWork : IUnitOfWorkRoot
    {
        IFeatureRepository Features { get; }
        IFeatureTypeRepository FeaturesTypes { get; }

        IProductRepository Products { get; }
        IProductTypeRepository ProductTypes { get; }

        IOrderRepository Orders { get; }
    }
}
