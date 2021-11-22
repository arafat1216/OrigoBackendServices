using ProductCatalog.Service.Infrastructure;
using ProductCatalog.Service.Infrastructure.Context;

namespace ProductCatalog.Service
{
    public class ProductService
    {
        //private readonly ProductCatalogContext _context;
        private readonly UnitOfWork _unitOfWork;

        public ProductService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            //_context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal ProductService(ProductCatalogContext context)
        {
            //_context = context;
            _unitOfWork = new UnitOfWork(context);
        }


    }
}
