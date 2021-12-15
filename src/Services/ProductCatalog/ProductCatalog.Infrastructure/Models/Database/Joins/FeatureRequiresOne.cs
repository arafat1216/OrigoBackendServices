using ProductCatalog.Infrastructure.Models.Database;

namespace FeatureCatalog.Infrastructure.Models.Database.Joins
{
    /// <summary>
    ///     A DB join-table that keeps track of the requirements / system-rules for a <see cref="Feature"/>. 
    ///     AT LEAST ONE of the required features must exist (or be added) before a given feature is valid and can be added/used.
    /// </summary>
    internal class FeatureRequiresOne : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The feature that has a requirement.
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        ///     The feature that is required. The <see cref="FeatureId"/> requires at least one corresponding feature to be present before
        ///     it can be added or used. This is a one-way requirement.
        /// </summary>
        public int RequiresFeatureId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Feature? Feature { get; set; }
        public virtual Feature? RequiresFeature { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureRequiresOne() : base()
        {
        }
    }
}
