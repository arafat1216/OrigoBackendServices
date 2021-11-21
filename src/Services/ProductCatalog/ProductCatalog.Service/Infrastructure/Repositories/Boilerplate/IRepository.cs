using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IDbEntity, new()
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
