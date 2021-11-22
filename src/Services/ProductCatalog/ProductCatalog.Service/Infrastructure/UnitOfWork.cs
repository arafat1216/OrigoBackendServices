using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Infrastructure.Repositories;

namespace ProductCatalog.Service.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ProductCatalogContext _context;

        public IFeatureRepository Features { get; private set; }
        public IProductRepository Products { get; private set; }
        public IOrderRepository Orders { get; private set; }


        public UnitOfWork(ProductCatalogContext context)
        {
            _context = context;

            Features = new FeatureRepository(context);
            Products = new ProductRepository(context);
            Orders = new OrderRepository(context);
        }


        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
