namespace ProductCatalog.Service.Models.Database
{
    public class Order : Entity
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ProductId { get; set; }
        public Guid OrganizationId { get; set; }

        // EF Navigation
        public virtual Product Product { get; set; }
    }
}
