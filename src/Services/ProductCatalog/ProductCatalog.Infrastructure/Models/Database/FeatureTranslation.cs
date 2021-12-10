using ProductCatalog.Infrastructure.Models.Boilerplate;

namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class FeatureTranslation : Translation
    {
        // EF DB Columns
        public int FeatureId { get; set; }


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public FeatureTranslation() : base()
        {
        }


        public FeatureTranslation(string language, string name, string? description, Guid updatedBy) : base(language, name, description, updatedBy)
        {
        }
    }
}
