using ProductCatalog.Service.Models.Database.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    /// <summary>
    ///     Defines the basic read operations that is shared across all repositories.
    /// </summary>
    /// <typeparam name="TEntity"> A entity-class that is registered with, and recognized by the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> currently 
    ///     used in the repository. The class must be added as a <see cref="Microsoft.EntityFrameworkCore.DbSet{TEntity}"/>, must implement 
    ///     the <see cref="IEntityFrameworkEntity"/> interface, and will be used for all the database operations. </typeparam>
    /// <seealso cref="ITemporalRepository{TEntity}"/>
    /// <seealso cref="IRepository{TEntity}"/>
    /// <seealso cref="ITranslationRepository{TEntity}"/>
    internal interface IReadRepository<TEntity> where TEntity : class, IEntityFrameworkEntity
    {
        /// <summary>
        ///     Finds an entity with the given primary key values. If no entity is found, then <see langword="null"/> is returned.
        /// </summary>
        /// <typeparam name="TId"> The datatype used by the key values. </typeparam>
        /// <param name="id"> The primary key values for the entity. </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> Returns the entity if it was found. Otherwise is returns <see langword="null"/>.</returns>
        Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns></returns>
        IAsyncEnumerable<TEntity> GetAllAsStream(bool asNoTracking);


        /// <summary>
        ///     
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> A enumerator containing all matching entities. </returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns></returns>
        IAsyncEnumerable<TEntity> FindAsStream(Expression<Func<TEntity, bool>> predicate, bool asNoTracking);


        /// <summary>
        ///     Asynchronously returns the number of matching elements in the database.
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the number of elements. </returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
