using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Service.Models.Database
{
    internal class ProductType : Entity, ITranslatable<ProductTypeTranslation>
    {
        // EF DB Columns
        public int Id { get; set; }

        // EF Owned
        public virtual ICollection<ProductTypeTranslation> Translations { get; set; }

        // EF Navigation
        public virtual ICollection<Product> Products { get; set; }


        [Obsolete("A reserved constructor that should only be utilized by EF!")]
        public ProductType()
        {
            Translations = new HashSet<ProductTypeTranslation>();
            Products = new HashSet<Product>();
        }


        public ProductType(ICollection<ProductTypeTranslation> translations)
        {
            Translations = translations;
        }

        public ProductType(IEnumerable<ProductTypeTranslation> translations)
        {
            Translations = translations.ToList();
        }

        public ProductType(IEnumerable<ITranslationResult> translations)
        {
            Translations = new EntityAdapter().Convert<ProductTypeTranslation>(translations).ToList();
        }
    }
}
