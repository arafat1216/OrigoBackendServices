using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    /// <typeparam name="TDbContext"> The <see cref="DbContext"/> that will be used by the repository. </typeparam>
    /// <inheritdoc cref="IReadRepository{TEntity}"/>
    internal abstract class Repository<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class, IEntityFrameworkEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;

        protected Repository(TDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>()
                                 .AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>()
                          .AddRangeAsync(entities, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TEntity> FindAsStream(Expression<Func<TEntity, bool>> predicate, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               .AsAsyncEnumerable();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TEntity> GetAllAsStream(bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .AsAsyncEnumerable();
            }
        }

        // Alternative to the "TId id" parameter => "params object?[]? keys"
        /// <inheritdoc/>
        public virtual async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
        {
            return await _context.Set<TEntity>()
                                 .FindAsync(new object[] { id }, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>()
                    .Remove(entity);
        }

        /// <inheritdoc/>
        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>()
                    .RemoveRange(entities);
        }

        /// <inheritdoc/>
        public virtual void Update(TEntity entity)
        {
            throw new NotImplementedException();

            // TODO: Read up on this in docs - https://docs.microsoft.com/en-us/ef/core/change-tracking/
            _context.Set<TEntity>()
                    .Update(entity);

        }

        /// <inheritdoc/>
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>()
                                 .CountAsync(cancellationToken);
        }




    }
}
