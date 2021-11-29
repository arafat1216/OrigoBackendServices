using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Models.Database
{
    internal class ProductTypeTranslation : Translation
    {
        // EF DB Columns
        public int ProductTypeId { get; set; }

        [Obsolete("A reserved constructor that should only be utilized by EF!")]
        public ProductTypeTranslation()
        {
        }

        public ProductTypeTranslation(string language, string name, string? description)
        {
            Language = language;
            Name = name;
            Description = description;
        }

        public ProductTypeTranslation(int productTypeId, string language, string name, string? description)
        {
            ProductTypeId = productTypeId;
            Language = language;
            Name = name;
            Description = description;
        }
    }
}
