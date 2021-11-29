using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Infrastructure.Repositories;

namespace ProductCatalog.Service.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ProductCatalogContext _context;

        public IFeatureRepository Features { get; private set; }
        public IFeatureTypeRepository FeaturesTypes { get; private set; }
        public IProductRepository Products { get; private set; }
        public IProductTypeRepository ProductTypes { get; private set; }
        public IOrderRepository Orders { get; private set; }

        public UnitOfWork(ProductCatalogContext context)
        {
            _context = context;

            Features = new FeatureRepository(_context);
            FeaturesTypes = new FeatureTypeRepository(_context);
            Products = new ProductRepository(_context);
            ProductTypes = new ProductTypeRepository(_context);
            Orders = new OrderRepository(_context);
        }

        /// <summary>
        ///     Saves all changes made to the database.
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database. </returns>
        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


        #region IDisposable / IAsyncDisposable interface implementations

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Dispose"/>
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        /// <seealso cref="DisposeAsync"/>
        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}
