using ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IFeatureRepository : ITranslationRepository<Feature>
    {
        Task<IEnumerable<string>> GetPermissionNodesByOrganizationAsync(Guid organizationId);
    }
}
