using System.Linq.Expressions;

namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     Defines all non-temporal read-methods for the repositories.
    /// </summary>
    /// <typeparam name="TEntity"> The entity-class to be used for all EntityFramework / database operations. The entity-class must be recognized by 
    ///     the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> used by the repository, and also has to be a valid and registered
    ///     <see cref="Microsoft.EntityFrameworkCore.DbSet{TEntity}"/> entry within this context. </typeparam>
    public interface IReadRepository<TEntity> : IRepositoryRoot
                                                where TEntity : class
    {
        /// <summary>
        ///     Retrieves all items from the database. The optional parameters can be used to further refine the query and result by applying filters, 
        ///     includes, sorting/ordering, as well as adding page-based data retrieval.
        /// </summary>
        /// <param name="filter"> A lambda-expression used to apply a 'where' filters on the retrieved dataset. If this is <see langword="null"/> then
        ///     no conditional filters is applied. </param>
        /// <param name="orderBy"> A lambda-expression used to define the ordering of the results. If this is <see langword="null"/> the database's
        ///     default ordering is used. </param>
        /// <param name="includeProperties"></param>
        /// <param name="pageIndex"> The current page number to be retrieved. If this is <see langword="null"/>, then all results are returned, 
        ///     without using pagination. </param>
        /// <param name="pageSize"> The number of items to include per page. This is only used when <paramref name="pageIndex"/> is not <see langword="null"/>. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> An enumerator containing all matching results. </returns>
        public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                   string includeProperties = "",
                                                   int? pageIndex = null,
                                                   int pageSize = 50,
                                                   bool asNoTracking = false,
                                                   CancellationToken cancellationToken = default
        );


        /// <summary>
        ///     Retrieve a single entity using it's primary key(s).
        /// </summary>
        /// <typeparam name="T"> The data-type used for the primary key(s). </typeparam>
        /// <param name="id"> The primary key(s) for the entity. </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> If found, the matching entity. Otherwise it returns <see langword="null"/>. </returns>
        public Task<TEntity?> GetByIdAsync<T>(T id, CancellationToken cancellationToken = default) where T : notnull;


        /// <summary>
        ///     Returns the number of items recorded in the database for the given entity. The optional parameters can be utilized to further 
        ///     refine the query and result by applying a filter.
        /// </summary>
        /// <param name="filter"> A lambda-expression used to apply a 'where' filters on the retrieved dataset. If this is <see langword="null"/> then
        ///     no conditional filters is applied. </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
                             CancellationToken cancellationToken = default
        );
    }
}
