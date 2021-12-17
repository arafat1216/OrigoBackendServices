using OrigoApiGateway.Models.ProductCatalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IProductCatalogServices
    {
        #region Features

        /// <summary>
        ///     Retrieves a list containing every product specific permissions-node that is currently available
        ///     for the given <paramref name="organizationId"/>.
        /// </summary>
        /// <param name="organizationId"> The organization to retrieve permissions for. </param>
        /// <returns> A list containing the organizations current permission-nodes. </returns>
        Task<IEnumerable<string>> GetProductPermissionsForOrganizationAsync(Guid organizationId);

        #endregion


        #region Products

        /// <summary>
        ///     Retrieves a <see cref="ProductGet">product</see> using it's ID.
        /// </summary>
        /// <param name="productId"> The products ID. </param>
        /// <returns> The corresponding product, if found. </returns>
        Task<ProductGet> GetProductByIdAsync(int productId);

        /// <summary>
        ///     Retrieves all products that belongs to the provided partner.
        /// </summary>
        /// <param name="partnerId"> The partners ID. </param>
        /// <returns> A list containing all matching <see cref="ProductGet">products</see>. </returns>
        Task<IEnumerable<ProductGet>> GetAllProductsByPartnerAsync(Guid partnerId);

        #endregion


        #region Product Types

        /// <summary>
        ///     Retrieves a list off all <see cref="ProductTypeGet">product types</see>.
        /// </summary>
        /// <returns> A list containing all <see cref="ProductTypeGet">product types</see>. </returns>
        Task<IEnumerable<ProductTypeGet>> GetAllProductTypesAsync();

        #endregion


        #region Orders

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

        #endregion
    }
}
