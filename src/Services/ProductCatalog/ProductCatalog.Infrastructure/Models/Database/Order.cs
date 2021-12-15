namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Represents a single order. This is used to keep track of the <see cref="Product">products</see> that is currently purchased/active on a given organization.
    /// </summary>
    internal class Order : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The primary key. This is the internal key, and should not be exposed through the APIs.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     The external ID. This is the ID that is used by the endpoints and other external sources.
        /// </summary>
        public Guid ExternalId { get; private set; }

        /// <summary>
        ///     The foreign-key to the <see cref="Product"/>.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The organization the order belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Product? Product { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public Order() : base()
        {
        }


        public Order(int productId, Guid organizationId, Guid updatedBy) : base(updatedBy)
        {
            ProductId = productId;
            OrganizationId = organizationId;
        }
    }
}
