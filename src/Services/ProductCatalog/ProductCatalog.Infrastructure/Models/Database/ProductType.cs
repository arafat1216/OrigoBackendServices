using ProductCatalog.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProductCatalog.Infrastructure.Models.Database
{
    internal class ProductType : Entity, ITranslatable<ProductTypeTranslation>
    {
        // EF DB Columns
        public int Id { get; set; }

        // EF Owned
        public virtual ICollection<ProductTypeTranslation> Translations { get; set; }

        // EF Navigation
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public ProductType() : base()
        {
            Translations = new HashSet<ProductTypeTranslation>();
            Products = new HashSet<Product>();
        }


        public ProductType(ICollection<ProductTypeTranslation> translations, Guid updatedBy) : base(updatedBy)
        {
            Translations = translations;
        }

        public ProductType(IEnumerable<ProductTypeTranslation> translations, Guid updatedBy) : base(updatedBy)
        {
            Translations = translations.ToList();
        }

        public ProductType(IEnumerable<ITranslationResult> translations, Guid updatedBy) : base(updatedBy)
        {
            Translations = new EntityAdapter().Convert<ProductTypeTranslation>(translations).ToList();
        }
    }
}
