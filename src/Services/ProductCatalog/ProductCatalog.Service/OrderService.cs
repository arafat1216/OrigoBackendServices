using ProductCatalog.Service.Infrastructure;
using ProductCatalog.Service.Infrastructure.Context;

namespace ProductCatalog.Service
{
    public class OrderService
    {
        private readonly UnitOfWork _unitOfWork;

        public OrderService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal OrderService(ProductCatalogContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }



    }
}
