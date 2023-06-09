﻿namespace ProductCatalog.Infrastructure.Models.Database.Joins
{
    /// <summary>
    ///     A DB join-table that keeps track of the requirements / system-rules for a <see cref="Product"/>. 
    ///     NONE of these excluded products can exist (or be added) if the provided product is added/used.
    /// </summary>
    internal class ProductExcludes : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The product that has exclusions.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        ///     The product that is excluded.
        /// </summary>
        public int ExcludesProductId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Product? Product { get; set; }
        public virtual Product? RequiresProduct { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public ProductExcludes() : base()
        {
        }
    }
}
