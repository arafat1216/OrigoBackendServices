namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    internal class ProductExcludes
    {
        // EF DB Columns

        /// <summary>
        ///     The product that has exclusions.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The product that is excluded.
        /// </summary>
        public int ExcludesProductId { get; set; }

        // EF Navigation

        public virtual Product? Product { get; set; }
        public virtual Product? RequiresProduct { get; set; }
    }
}
