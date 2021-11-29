namespace ProductCatalog.Service.Models.Database
{
    internal class ProductFeature : Entity
    {
        // EF DB Columns
        public int FeatureId { get; set; }
        public int ProductId { get; set; }

        // EF Navigation
        public virtual Feature? Feature { get; set; }
        public virtual Product? Product { get; set; }
    }
}
