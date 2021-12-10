using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     The abstract root-class utilized by every repository.
    /// </summary>
    /// <typeparam name="TDbContext"> A derivative <see cref="DbContext"/>-class that will be used for all database-interactions by the repository. </typeparam>
    /// <typeparam name="TEntity"> The entity-class to be used for all EntityFramework / database operations. The entity-class must be registered as 
    ///     a <see cref="DbSet{TEntity}"/> by the <typeparamref name="TDbContext"/> that is used by the repository. </typeparam>
    public abstract class RepositoryRoot<TDbContext, TEntity> where TDbContext : DbContext
                                                              where TEntity : class

    {
        /// <summary> A derivative <see cref="DbContext"/>-class that is used for all database-interactions. </summary>
        protected readonly TDbContext _context;

        /// <summary> The current <see cref="DbSet{TEntity}"/> entity-class that is used for all queries. </summary>
        protected readonly DbSet<TEntity> _dbSet;

        protected readonly ILogger? _logger;


        protected RepositoryRoot(ref TDbContext context, ref ILogger logger)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _logger = logger;
        }


        protected RepositoryRoot(ref TDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

    }
}