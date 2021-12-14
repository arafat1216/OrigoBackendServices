using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public abstract class TemporalReadWriteRepository<TDbContext, TEntity> : ReadWriteRepository<TDbContext, TEntity>,
                                                                             ITemporalReadWriteRepository<TEntity>
                                                                             where TDbContext : DbContext
                                                                             where TEntity : class
    {
        public ITemporalRepositoryRoot<TEntity> Temporal { get; private set; }


        protected TemporalReadWriteRepository(ref TDbContext context, ref ILogger logger) : base(ref context, ref logger)
        {
            Temporal = new TemporalRepositoryRoot<TDbContext, TEntity>(ref context, ref logger);
        }


        protected TemporalReadWriteRepository(ref TDbContext context) : base(ref context)
        {
            Temporal = new TemporalRepositoryRoot<TDbContext, TEntity>(ref context);
        }

    }
}
