using ProductCatalog.Domain.Exceptions;
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


        public async Task<IEnumerable<OrderGet>> GetOrders(Guid? organizationId, Guid? partnerId)
        {
            IEnumerable<Order>? result;

            if (organizationId is null && partnerId is null) // Both are null
                result = await _unitOfWork.Orders.GetAsync();
            else if (partnerId is null) // One is null, so the other one isn't
                result = await _unitOfWork.Orders.GetAsync(e => e.OrganizationId == organizationId);
            else if (organizationId is null) // One is null, so the other one isn't
                result = await _unitOfWork.Orders.GetAsync(e => e.Product!.PartnerId == partnerId);
            else
                result = await _unitOfWork.Orders.GetAsync(e => e.OrganizationId == organizationId && e.Product!.PartnerId == partnerId);

            return new EntityAdapter().ToDTO(result);
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

        /// <summary>
        ///     Validates the requirements before updating the list of ordered products for a organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="updateProductOrders"> The </param>
        /// <returns></returns>
        /// <exception cref="RequirementNotFulfilledException"> Thrown when one or more of the product requirements failed to pass their checks.
        ///     This typically means we are missing a dependency product, or that an excluded product have been added. </exception>
        /// <exception cref="EntityNotFoundException"> <inheritdoc cref="ProductService.ValidateProductListRequirements(IEnumerable{int}, Guid)"/> </exception>
        public async Task UpdateOrderedProductsAsync(Guid organizationId, Guid partnerId, UpdateProductOrders updateProductOrders)
        {
            // Make sure the configuration is valid before we do anything
            var validConfiguration = await new ProductService().ValidateProductListRequirements(updateProductOrders.ProductIds, partnerId);
            if (!validConfiguration)
                throw new RequirementNotFulfilledException("One or more requirement checks failed. Make sure no requirements are missing, and that no excluded items have been added.");


            var currentProducts = await _unitOfWork.Orders.GetAsync(filter: e => e.OrganizationId == organizationId);

            var currentProductIds = from i in currentProducts
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

            await _unitOfWork.SaveAsync();
        }
    }
}
