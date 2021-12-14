using ProductCatalog.Infrastructure.Models.Database;

namespace FeatureCatalog.Infrastructure.Models.Database.Joins
{
    /// <summary>
    ///     A DB join-table that keeps track of the requirements / system-rules for a <see cref="Feature"/>. 
    ///     NONE of these excluded features can exist (or be added) if the provided feature is added/used.
    /// </summary>
    internal class FeatureExcludes : Entity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The feature that has exclusion-requirements.
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        ///     The feature that is excluded. The <see cref="FeatureId"/> can't be combined or used alongside the any of these features. This is a one-way requirement.
        /// </summary>
        public int ExcludesFeatureId { get; set; }

        /*
         * EF Navigation
         */

        public virtual Feature? Feature { get; set; }
        public virtual Feature? RequiresFeature { get; set; }

        /*
         * Constructor
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureExcludes() : base()
        {
        }
    }
}
