using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public sealed class TemporalRepositoryRoot<TDbContext, TEntity> : RepositoryRoot<TDbContext, TEntity>,
                                                                      ITemporalRepositoryRoot<TEntity>
                                                                      where TDbContext : DbContext
                                                                      where TEntity : class
    {

        // Must be instantiated by one of the other pre-defined generic repositories
        internal protected TemporalRepositoryRoot(ref TDbContext context, ref ILogger logger) : base(ref context, ref logger)
        {
        }


        internal protected TemporalRepositoryRoot(ref TDbContext context) : base(ref context)
        {
        }

    }
}
