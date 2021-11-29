using ProductCatalog.Service.Infrastructure.Repositories.Boilerplate;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IFeatureRepository : ITranslationRepository<Feature>
    {
        Task<IEnumerable<string>> GetPermissionNodesByOrganizationAsync(Guid organizationId);
    }
}
