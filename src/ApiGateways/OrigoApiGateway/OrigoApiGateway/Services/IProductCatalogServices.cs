using OrigoApiGateway.Models.ProductCatalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IProductCatalogServices
    {
        // Features
        Task<IEnumerable<string>> GetProductPermissionsForOrganizationAsync(Guid organizationId);

        // Products
        Task<ProductGet> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductGet>> GetAllProductsByPartnerAsync(Guid partnerId);

        // Product Types
        Task<IEnumerable<ProductTypeGet>> GetAllProductTypesAsync();

        // Orders
        Task<IEnumerable<ProductGet>> GetOrdersByPartnerAndOrganizationAsync(Guid organizationId);
        Task ReplaceOrderedProductsAsync(Guid partnerId, Guid organizationId, ProductOrdersDTO updateProductOrders);
    }
}
