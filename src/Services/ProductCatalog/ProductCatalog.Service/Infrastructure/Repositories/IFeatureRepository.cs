using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IFeatureRepository : IRepository<Feature>
    {
        Task<IEnumerable<string>> GetPermissionNodesByOrganizationAsync(Guid organizationId);
    }
}
