using ProductCatalog.Service.Infrastructure.Repositories;

namespace ProductCatalog.Service.Infrastructure
{
    internal interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IFeatureRepository Features { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }

        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
