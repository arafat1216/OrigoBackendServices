using ProductCatalog.Infrastructure.Models.Boilerplate;

namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class FeatureTypeTranslation : Translation
    {
        /*
         * EF DB Columns
         */

        public int FeatureTypeId { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureTypeTranslation() : base()
        {
        }


        public FeatureTypeTranslation(string language, string name, string? description, Guid updatedBy) : base(language, name, description, updatedBy)
        {
        }
    }
}
