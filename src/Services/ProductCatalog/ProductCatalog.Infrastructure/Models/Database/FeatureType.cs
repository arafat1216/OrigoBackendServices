using ProductCatalog.Common.Interfaces;

namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class FeatureType : Entity, ITranslatable<FeatureTypeTranslation>
    {
        /*
         * EF DB Columns
         */

        public int Id { get; init; }

        /*
         * EF Owned Tables
         */

        public virtual ICollection<FeatureTypeTranslation> Translations { get; set; } = new HashSet<FeatureTypeTranslation>();

        /*
         * EF Navigation
         */

        public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureType() : base()
        {
        }


        public FeatureType(ICollection<FeatureTypeTranslation> translations, Guid updatedBy) : base(updatedBy)
        {
            Translations = translations;
        }
    }
}
