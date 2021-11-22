namespace ProductCatalog.Service.Models.Database
{
    internal class ProductFeature : Entity
    {
        public int FeatureId { get; set; }
        public int ProductId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Product Product { get; set; }
    }
}
