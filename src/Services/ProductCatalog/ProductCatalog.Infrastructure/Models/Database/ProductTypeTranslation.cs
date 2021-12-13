using ProductCatalog.Infrastructure.Models.Boilerplate;

namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class ProductTypeTranslation : Translation
    {
        /*
         * EF DB Columns
         */

        public int ProductTypeId { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public ProductTypeTranslation() : base()
        {
        }

        public ProductTypeTranslation(string language, string name, string? description, Guid updatedBy) : base(language, name, description, updatedBy)
        {
        }

        public ProductTypeTranslation(int productTypeId, string language, string name, string? description, Guid updatedBy) : base(language, name, description, updatedBy)
        {
            ProductTypeId = productTypeId;
        }
    }
}
