namespace Boilerplate.EntityFramework.Generics.UnitOfWork
{
    /// <summary>
    ///     Defines all methods implemented by <see cref="UnitOfWorkRoot{TDbContext}"/>.
    /// </summary>
    public interface IUnitOfWorkRoot : IDisposable
    {
        /// <summary>
        ///     Saves all changes to the database.
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> A task that represents the asynchronous save operation. The task result contains the number of entries written to the database. </returns>
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
