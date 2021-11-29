using ProductCatalog.Service.Infrastructure.Repositories;

namespace ProductCatalog.Service.Infrastructure
{
    internal interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IFeatureRepository Features { get; }
        IFeatureTypeRepository FeaturesTypes { get; }

        IProductRepository Products { get; }
        IProductTypeRepository ProductTypes { get; }

        IOrderRepository Orders { get; }

        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
