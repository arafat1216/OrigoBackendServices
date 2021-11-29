using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Products
{
    public class ProductTypeGet
    {
        internal ProductTypeGet(int id, IEnumerable<Translation> translations)
        {
            Id = id;
            Translations = translations;
        }

        public ProductTypeGet(int id, IEnumerable<ITranslationResult> translations)
        {
            var results = new TypeConverter().ToTranslation(translations);

            Id = id;
            Translations = results;
        }


        [Required]
        public int Id { get; }

        [Required]
        public IEnumerable<Translation> Translations { get; }
    }
}
