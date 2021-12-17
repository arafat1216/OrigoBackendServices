using OrigoApiGateway.Models.ProductCatalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IProductCatalogServices
    {
        /*
         * Features
         */

        Task<IEnumerable<string>> GetProductPermissionsForOrganizationAsync(Guid organizationId);

        /*
         * Products
         */

        Task<ProductGet> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductGet>> GetAllProductsByPartnerAsync(Guid partnerId);

        /*
         * Product Types
         */

        Task<IEnumerable<ProductTypeGet>> GetAllProductTypesAsync();

        /*
         * Orders
         */

        /// <summary>
        ///     Retrieves all products that has been ordered by an organization, filtered by a partner-ID.
        /// </summary>
        /// <param name="partnerId"> The partner to retrieve orders for. </param>
        /// <param name="organizationId"> The organization to retrieve orders for. </param>
        /// <returns> A collection containing all corresponding products. </returns>
        Task<IEnumerable<ProductGet>> GetOrderedProductsByPartnerAndOrganizationAsync(Guid partnerId, Guid organizationId);


        /// <summary>
        ///     Updates all current product-orders for a given organization, and replaces them with a new configuration. This only affects products 
        ///     for the provided <paramref name="partnerId"/>, and will not alter products that is purchased from other partners.
        /// </summary>
        /// <param name="partnerId"> The partner that is placing the order. </param>
        /// <param name="organizationId"> The organization that is updated with a new product-configuration. </param>
        /// <param name="productOrders"> The object that contains the new order-details. </param>
        /// <returns> The <see langword="async"/> <see cref="Task"/>. </returns>
        Task ReplaceOrderedProductsAsync(Guid partnerId, Guid organizationId, ProductOrdersDTO productOrders);
    }
}
