using Boilerplate.EntityFramework.Generics.Repositories;
using ProductCatalog.Infrastructure.Models.Database;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories
{
    internal interface IOrderRepository : ITemporalReadWriteRepository<Order>
    {
        /// <summary>
        ///     Retrieves a list of all purchased product-IDs. This list contains the results belonging to a single organization, 
        ///     a single partner, or a combination of both. At least one of these parameters must be supplied! 
        /// </summary>
        /// <param name="organizationId"> Filter the orders so we only return products purchased by this organization. If this is <see langword="null"/>, 
        ///     then <paramref name="partnerId"/> must be supplied, and no organization filters will be added. </param>
        /// <param name="partnerId"> Filter orders so we only return products belonging to this partner. If this parameter is <see langword="null"/>,
        ///     then <paramref name="organizationId"/> must be supplied, and no partner filter will be added. </param>
        /// <returns></returns>
        //Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(Guid? organizationId, Guid? partnerId);
        Task<IEnumerable<int>> GetProductIdsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null);

        /// <summary>
        ///     Retrieves a list of all purchased products. This list contains the results belonging to a single organization, 
        ///     a single partner, or a combination of both. At least one of these parameters must be supplied! 
        /// </summary>
        /// <param name="organizationId"> Filter the orders so we only return products purchased by this organization. If this is <see langword="null"/>, 
        ///     then <paramref name="partnerId"/> must be supplied, and no organization filters will be added. </param>
        /// <param name="partnerId"> Filter orders so we only return products belonging to this partner. If this parameter is <see langword="null"/>,
        ///     then <paramref name="organizationId"/> must be supplied, and no partner filter will be added. </param>
        /// <returns> A list of products that matches the provided parameters. </returns>
        //Task<IEnumerable<Product>> GetProductsFromOrdersAsync(Guid? organizationId, Guid? partnerId);
        Task<IEnumerable<Product>> GetProductsFromOrdersAsync(Expression<Func<Order, bool>>? filter = null);
    }
}
