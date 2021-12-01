namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    internal class ProductRequiresAll
    {
        // EF DB Columns

        /// <summary>
        ///     The product that has requirements.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The product that is required.
        /// </summary>
        public int RequiresProductId { get; set; }

        // EF Navigation

        public virtual Product? Product { get; set; }
        public virtual Product? RequiresProduct { get; set; }
    }
}
