namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class Order : Entity
    {
        // EF DB Columns
        public int Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public int ProductId { get; set; }
        public Guid OrganizationId { get; set; }

        // EF Navigation
        public virtual Product? Product { get; set; }


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
