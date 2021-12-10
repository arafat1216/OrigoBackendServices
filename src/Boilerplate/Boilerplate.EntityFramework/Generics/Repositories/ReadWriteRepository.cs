using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public abstract class ReadWriteRepository<TDbContext, TEntity> : ReadRepository<TDbContext, TEntity>,
                                                                     IReadWriteRepository<TEntity>
                                                                     where TDbContext : DbContext
                                                                     where TEntity : class
    {
        protected ReadWriteRepository(ref TDbContext context, ref ILogger logger) : base(ref context, ref logger)
        {
        }

        protected ReadWriteRepository(ref TDbContext context) : base(ref context)
        {
        }


        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity);
        }


        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }



        public void AddOrUpdate(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }


        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            foreach (var entity in entitiesToUpdate)
            {
                Update(entity);
            }
        }


        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);
        }

        public virtual async Task Delete<T>(T id) where T : notnull
        {
            TEntity? entityToDelete = await GetByIdAsync(id);

            if (entityToDelete is not null)
            {
                Delete(entityToDelete);
            }
        }


        public virtual void DeleteRange(IEnumerable<TEntity> entitiesToDelete)
        {
            _dbSet.RemoveRange(entitiesToDelete);
        }
    }
}
