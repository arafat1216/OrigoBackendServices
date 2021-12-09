namespace ProductCatalog.Common.Orders
{
    /// <summary>
    ///     Represents a single, existing order.
    /// </summary>
    public class OrderGet
    {
        /// <summary>
        ///     The external ID for this order.
        /// </summary>
        public Guid ExternalId { get; private set; }

        /// <summary>
        ///     The ID for the ordered product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The organization that have placed the order.
        /// </summary>
        public Guid OrganizationId { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderGet"/> class.
        /// </summary>
        /// <param name="externalId"> The external ID for this order. </param>
        /// <param name="productId"> The ID for the ordered product. </param>
        /// <param name="organizationId"> The organization that have placed the order. </param>
        public OrderGet(Guid externalId, int productId, Guid organizationId)
        {
            ExternalId = externalId;
            ProductId = productId;
            OrganizationId = organizationId;
        }
    }
}
