using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<int>> GetProductIdsByOrganization(Guid organizationId);
    }
}
