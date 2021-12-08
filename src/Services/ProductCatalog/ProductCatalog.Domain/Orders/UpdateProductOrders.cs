using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Orders
{
    /// <summary>
    ///     A domain entity that represents a new product-configuration order for an organization.
    /// </summary>
    public class UpdateProductOrders
    {
        /// <summary>
        ///     The customers new and complete list of product IDs. Any existing products that's not added in this list will be removed.
        /// </summary>
        /// <example>
        ///     [1,20,54,55,875]
        /// </example>
        [Required]
        public IEnumerable<int> ProductIds { get; set; }


        /// <summary>
        ///     The ID of the user that performed the update.
        /// </summary>
        [Required]
        public Guid UpdatedBy { get; set; }


        public UpdateProductOrders(IEnumerable<int> productIds, Guid updatedBy)
        {
            ProductIds = productIds;
            UpdatedBy = updatedBy;
        }
    }
}
