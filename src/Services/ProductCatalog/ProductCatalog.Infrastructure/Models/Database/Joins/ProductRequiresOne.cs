namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    /// <summary>
    ///     A DB join-table that keeps track of the requirements / system-rules for a <see cref="Product"/>. 
    ///     AT LEAST ONE of the required products must exist (or be added) before a given product is valid and can be added/used.
    /// </summary>
    internal class ProductRequiresOne : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The product that has requirements.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The product that is required.
        /// </summary>
        public int RequiresProductId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Product? Product { get; set; }
        public virtual Product? RequiresProduct { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public ProductRequiresOne() : base()
        {
        }
    }
}
