using ProductCatalog.Domain.Orders;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.ProductTypes;
using ProductCatalog.Infrastructure.Infrastructure;
using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Models.Database;
namespace ProductCatalog.Infrastructure
{
    public class OrderService
    {
        private readonly UnitOfWork _unitOfWork;

        public OrderService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal OrderService(ProductCatalogContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }


        // TODO: Rework later to support partners (so a partner that's adding or removing products can't access anything that's purchased through another partner)
        public async Task<IEnumerable<int>> GetOrderedProductIdsAsync(Guid? organizationId, Guid? partnerId)
        {
            if (organizationId is null && partnerId is null) // Both are null
                return await _unitOfWork.Orders.GetProductIdsFromOrdersAsync();
            else if (partnerId is null) // One is null, so the other one isn't
                return await _unitOfWork.Orders.GetProductIdsFromOrdersAsync(e => e.OrganizationId == organizationId);
            else if (organizationId is null) // One is null, so the other one isn't
                return await _unitOfWork.Orders.GetProductIdsFromOrdersAsync(e => e.Product!.PartnerId == partnerId);
            else // None are null
                return await _unitOfWork.Orders.GetProductIdsFromOrdersAsync(e => e.Product!.PartnerId == partnerId && e.OrganizationId == organizationId);
        }


        // TODO: Rework later to support partners (so a partner that's adding or removing products can't access anything that's purchased through another partner)
        public async Task<IEnumerable<ProductGet>> GetOrderedProductsAsync(Guid? organizationId, Guid? partnerId)
        {
            IEnumerable<Product>? result;

            if (organizationId is null && partnerId is null) // Both are null
                result = await _unitOfWork.Orders.GetProductsFromOrdersAsync();
            else if (partnerId is null) // One is null, so the other one isn't
                result = await _unitOfWork.Orders.GetProductsFromOrdersAsync(e => e.OrganizationId == organizationId);
            else if (organizationId is null) // One is null, so the other one isn't
                result = await _unitOfWork.Orders.GetProductsFromOrdersAsync(e => e.Product!.PartnerId == partnerId);
            else // None are null
                result = await _unitOfWork.Orders.GetProductsFromOrdersAsync(e => e.Product!.PartnerId == partnerId && e.OrganizationId == organizationId);

            return new EntityAdapter().ToDTO(result);
        }


        public async Task UpdateOrderedProductsAsync(Guid organizationId, UpdateProductOrders updateProductOrders)
        {
            var currentProducts = await _unitOfWork.Orders.GetAsync(filter: e => e.OrganizationId == organizationId);

            if (updateProductOrders.ProductIds.Count() != currentProducts.Count())
                throw new ArgumentException("One or more products was not found.");


            var currentProductIds = from i
                                    in currentProducts
                                    select i.ProductId;

            // Find all IDs that's exists, but is not present in the new product-list
            var toBeRemoved = from order in currentProducts
                              where !updateProductOrders.ProductIds.Contains(order.ProductId)
                              select order;

            // Find all IDs that is in the new product-list, but is not present in list of currently added products.
            var toBeAdded = from productId in updateProductOrders.ProductIds
                            where !currentProductIds.Contains(productId)
                            select productId;

            _unitOfWork.Orders.DeleteRange(toBeRemoved);

            foreach (var productId in toBeAdded)
            {
                var order = new Order(productId, organizationId, updateProductOrders.UpdatedBy);
                await _unitOfWork.Orders.AddAsync(order);
            }

            // TODO: Remove this line
            ValidateRequirements(updateProductOrders.ProductIds);

            await _unitOfWork.SaveAsync();
        }


        public async void ValidateRequirements(IEnumerable<int> newProductIds)
        {
            var newProducts = await _unitOfWork.Products.GetAsync(filter: e => newProductIds.Contains(e.Id));

            var a = Guid.Empty;
        }
    }
}
