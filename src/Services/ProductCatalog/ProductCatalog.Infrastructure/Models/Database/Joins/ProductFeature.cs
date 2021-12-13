namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    /// <summary>
    ///     A DB join-table that keeps track of what <see cref="Feature">features</see> is added to the various <see cref="Product">products</see>.
    /// </summary>
    internal class ProductFeature : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     A foreign-key to the corresponding <see cref="FeatureType.Id"/>
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        ///     A foreign-key to the corresponding <see cref="Product.Id"/>
        /// </summary>
        public int ProductId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Feature? Feature { get; set; }
        public virtual Product? Product { get; set; }

        /*
         * Constructors
         */

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
