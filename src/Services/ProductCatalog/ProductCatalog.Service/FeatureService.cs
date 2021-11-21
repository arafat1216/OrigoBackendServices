using ProductCatalog.Service.Infrastructure;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service
{
    public class FeatureService
    {
        private readonly ProductCatalogContext _context;

        public FeatureService()
        {
            string[] args = Array.Empty<string>();
            _context = new ProductCatalogContextFactory().CreateDbContext(args);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal FeatureService(ProductCatalogContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Feature>> Test()
        {
            var work = new UnitOfWork(_context);

            return await work.Features.GetAllAsync();
        }

    }
}
