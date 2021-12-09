using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.ProductCatalog
{
    public class ProductOrdersDTO : ProductOrdersPut
    {
        /// <summary>
        ///     The ID of the user that performed the update
        /// </summary>
        public Guid UpdatedBy { get; set; }


        public ProductOrdersDTO() : base()
        {
        }

        public ProductOrdersDTO(IEnumerable<int> productIds, Guid updatedBy) : base(productIds)
        {
            UpdatedBy = updatedBy;
        }

        public ProductOrdersDTO(ProductOrdersPut productOrders, Guid updatedBy)
        {
            base.ProductIds = productOrders.ProductIds;
            this.UpdatedBy = updatedBy;
        }
    }
}
