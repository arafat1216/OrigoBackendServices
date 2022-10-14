using Boilerplate.EntityFramework.Generics.Repositories;
using ProductCatalog.Infrastructure.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IOrderRepository : ITemporalReadWriteRepository<Order>
    {
        /// <summary>
        ///     Retrieves a list of all <see cref="Product.Id">product-IDs</see> that has active orders.
        /// </summary>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <param name="filter"> An optional filter (where condition) that is applied to query. </param>
        /// <returns>
        ///     The asynchronous task. The task results contains a list of all <see cref="Product.Id">product IDs</see> that has active orders.
        /// </returns>
        Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(bool asNoTracking, Expression<Func<Order, bool>>? filter = null);

        /// <summary>
        ///     Retrieves a list of all <see cref="Product">products</see> that has active orders.
        /// </summary>
        /// <param name="includeTranslations"> If <see langword="true"/>, then the <see cref="Product.Translations"/>
        ///     list will be loaded and included in the results. </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <param name="filter"> An optional filter (where condition) that is applied to query. </param>
        /// <returns>
        ///     The asynchronous task. The task results contains a list of all <see cref="Product.Id">product IDs</see> that has active orders.
        /// </returns>
        Task<IEnumerable<Product>> GetProductsFromOrdersAsync(bool includeTranslations, bool asNoTracking, Expression<Func<Order, bool>>? filter = null);
    }
}
