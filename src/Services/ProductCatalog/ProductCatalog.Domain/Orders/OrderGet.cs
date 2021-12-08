namespace ProductCatalog.Domain.Orders
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
        ///     The ID for the ordered product
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The organization that have placed the order.
        /// </summary>
        public Guid OrganizationId { get; set; }


        public OrderGet(Guid externalId, int productId, Guid organizationId)
        {
            ExternalId = externalId;
            ProductId = productId;
            OrganizationId = organizationId;
        }
    }
}
