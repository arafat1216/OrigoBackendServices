using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal class FeatureRepository : TranslationRepository<ProductCatalogContext, Feature, FeatureTranslation>, IFeatureRepository
    {
        public FeatureRepository(ref ProductCatalogContext context) : base(ref context)
        {
        }

        public async Task<IEnumerable<string>> GetPermissionNodesByOrganizationAsync(Guid organizationId)
        {
            // 1. Join the "ProductFeature" (has the 'productId') and the "Feature" (has the 'AccessControlPermissionNode') tables.
            // 2. For a given organization, select all distinct product IDs (in case of overlapping IDs from several partners) using the ".Where()" clause.
            // 3. Using these results, we do a final distinct select for the feature permission node, based upon the returned product ID.
            var permissionNodes = await _context.ProductFeatures
                                                .Include(pf => pf.Feature)
                                                .Where(pf =>
                                                    _context.Orders
                                                            .Where(o => o.OrganizationId == organizationId)
                                                            .Select(o => o.ProductId)
                                                            .Distinct()
                                                            .Contains(pf.ProductId)
                                                 )
                                                .Select(pf => pf.Feature!.AccessControlPermissionNode)
                                                .Distinct()     // There is no reason to fetch duplicate permission nodes
                                                .AsNoTracking() // A read-only query, so we don't need to track changes (this increases speed)
                                                .ToListAsync(); // Microsoft's official "hack" to make the EntityFramework context support async
            return permissionNodes;
        }

    }
}
