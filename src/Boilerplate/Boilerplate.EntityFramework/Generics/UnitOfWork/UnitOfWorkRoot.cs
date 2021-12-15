using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boilerplate.EntityFramework.Generics.UnitOfWork
{
    /// <summary>
    ///     An abstract root-class for all <c>UnitOfWork</c> classes.
    /// </summary>
    /// <typeparam name="TDbContext"> The <see cref="DbContext"/> that is used by the repository. </typeparam>
    public abstract class UnitOfWorkRoot<TDbContext> : IUnitOfWorkRoot
                                                       where TDbContext : DbContext
    {
        /// <summary> The current <see cref="DbContext"/>. This must be the same as the one used by  </summary>
        private readonly TDbContext _context;
        private readonly ILogger? _logger;
        private bool disposedValue; // Implemented by the dispose-pattern


        // Let's temporarily remove this constructor until we actually use the logger.
        private UnitOfWorkRoot(ref TDbContext context, ref ILogger logger)
        {
            _context = context;
            _logger = logger;
        }


        protected UnitOfWorkRoot(ref TDbContext context)
        {
            _context = context;
        }


        /// <inheritdoc/>
        /// <exception cref="DbUpdateException"/>
        /// <exception cref="DbUpdateConcurrencyException"/>
        /// <exception cref="OperationCanceledException"/>
        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        #region Implementing 'IDisposable' with the default Dispose-pattern

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary> Releases the allocated resources. </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
