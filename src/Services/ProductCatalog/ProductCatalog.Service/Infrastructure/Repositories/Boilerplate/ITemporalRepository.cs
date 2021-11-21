using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface ITemporalRepository<TEntity> where TEntity : class, IDbEntity, new()
    {

    }
}
