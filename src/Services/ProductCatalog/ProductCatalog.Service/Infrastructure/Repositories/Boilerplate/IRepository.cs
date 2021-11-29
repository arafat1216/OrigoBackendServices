using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    /// <summary>
    ///     Defines the basic read and write operations that is shared across all repositories.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="IReadRepository{TEntity}"/>
    /// <seealso cref="ITemporalRepository{TEntity}"/>
    /// <seealso cref="ITranslationRepository{TEntity}"/>
    internal interface IRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IEntityFrameworkEntity
    {
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
