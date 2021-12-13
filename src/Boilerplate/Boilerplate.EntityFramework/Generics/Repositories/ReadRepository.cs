using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public abstract class ReadRepository<TDbContext, TEntity> : RepositoryRoot<TDbContext, TEntity>,
                                                                IReadRepository<TEntity>
                                                                where TDbContext : DbContext
                                                                where TEntity : class
    {

        protected ReadRepository(ref TDbContext context, ref ILogger logger) : base(ref context, ref logger)
        {
        }

        protected ReadRepository(ref TDbContext context) : base(ref context)
        {
        }

        /// <summary>
        ///     Constructs a dynamic EntityFramework query based upon several optional parameters.
        /// </summary>
        /// <returns> A dynamically created <see cref="IQueryable"/> that is ready to be consumed by EntityFramework. </returns>
        /// <exception cref="ArgumentException"> Thrown if the provided <see cref="int"/> value for <paramref name="pageIndex"/> or <paramref name="pageSize"/>
        ///     is a number that's lower then 1. </exception>
        /// <inheritdoc cref="GetAsync(Expression{Func{TEntity, bool}}?, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}?, string, int?, int, bool, CancellationToken)"/>
        protected IQueryable<TEntity> QueryBuilder(Expression<Func<TEntity, bool>>? filter = null,
                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                 string? includeProperties = null,
                                                 int? pageIndex = null,
                                                 int pageSize = 50,
                                                 bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            // Where filtering
            if (filter is not null)
                query = query.Where(filter);

            // Pagination
            if (pageIndex is not null)
            {
                if (pageIndex < 1)
                    throw new ArgumentException("Invalid page number. Must be 1 or higher.");
                if (pageIndex < 1)
                    throw new ArgumentException("Invalid page size. Must be 1 or higher.");

                query = query.Skip((pageIndex.Value - 1) * pageSize)
                             .Take(pageSize);
            }

            if (includeProperties is not null && includeProperties != string.Empty)
            {
                // Include properties
                foreach (var includedProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includedProperty);
                }
            }

            // Tracking
            if (asNoTracking)
                query = query.AsNoTracking();

            // Order by
            if (orderBy is not null)
                return orderBy(query);
            else
                return query;
        }


        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                                 string includeProperties = "",
                                                                 int? pageIndex = null,
                                                                 int pageSize = 50,
                                                                 bool asNoTracking = false,
                                                                 CancellationToken cancellationToken = default)
        {
            var query = QueryBuilder(filter, orderBy, includeProperties, pageIndex, pageSize, asNoTracking);

            return await query.ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        public virtual async Task<TEntity?> GetByIdAsync<T>(T id, CancellationToken cancellationToken = default) where T : notnull
        {
            return await _dbSet.FindAsync(id);
        }


        /// <inheritdoc/>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;

            // Where filtering
            if (filter is not null)
                query = query.Where(filter);

            return await query.AsNoTracking()
                              .CountAsync(cancellationToken);
        }
    }
}
