using ProductCatalog.Service.Models.Database.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal interface IReadRepository<TEntity> where TEntity : class, IDbEntity, new()
    {
        Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
