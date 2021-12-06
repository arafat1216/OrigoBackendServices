using ProductCatalog.Infrastructure.Models.Database;

namespace FeatureCatalog.Infrastructure.Models.Database.Joins
{
    internal class FeatureExcludes : Entity
    {
        // EF DB Columns

        /// <summary>
        ///     The feature that has exclusions.
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        ///     The feature that is excluded.
        /// </summary>
        public int ExcludesFeatureId { get; set; }

        // EF Navigation

        public virtual Feature? Feature { get; set; }
        public virtual Feature? RequiresFeature { get; set; }


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureExcludes() : base()
        {
        }
    }
}
