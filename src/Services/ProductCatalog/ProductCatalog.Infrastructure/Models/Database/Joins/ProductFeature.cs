namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    internal class ProductFeature : Entity
    {
        // EF DB Columns
        public int FeatureId { get; set; }
        public int ProductId { get; set; }

        // EF Navigation
        public virtual Feature? Feature { get; set; }
        public virtual Product? Product { get; set; }


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public ProductFeature() : base()
        {
        }


        public ProductFeature(int featureId, int productId, Guid updatedBy) : base(updatedBy)
        {
            FeatureId = featureId;
            ProductId = productId;
        }
    }
}
