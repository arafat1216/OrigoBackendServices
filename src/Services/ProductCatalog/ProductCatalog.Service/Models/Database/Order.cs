namespace ProductCatalog.Service.Models.Database
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
    }
}
