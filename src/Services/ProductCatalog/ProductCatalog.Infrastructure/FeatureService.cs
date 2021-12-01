using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Infrastructure;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure
{
    public class FeatureService
    {
        private readonly UnitOfWork _unitOfWork;

        public FeatureService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal FeatureService(ProductCatalogContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }


        public async Task<IEnumerable<string>> GetPermissionNodesAsync(Guid organizationId)
        {
            return await _unitOfWork.Features.GetPermissionNodesByOrganizationAsync(organizationId);
        }
        

    }
}
