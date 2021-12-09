using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.ProductCatalog
{
    /// <summary>
    ///     A domain entity that represents a new product-configuration order for an organization.
    /// </summary>
    public class ProductOrdersPut
    {
        /// <summary>
        ///     The customers new and complete list of product IDs. Any existing products that's not added in this list will be removed.
        /// </summary>
        /// <example>
        ///     [1,20,54,55,875]
        /// </example>
        [Required]
        public IEnumerable<int> ProductIds { get; set; }


        public ProductOrdersPut()
        {
            ProductIds = new List<int>();
        }


        public ProductOrdersPut(IEnumerable<int> productIds)
        {
            ProductIds = productIds;
        }
    }
}
