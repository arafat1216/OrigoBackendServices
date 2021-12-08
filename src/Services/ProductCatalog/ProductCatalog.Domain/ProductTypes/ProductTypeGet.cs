using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.ProductTypes
{
    /// <summary>
    ///     Represents a single, existing product-type.
    /// </summary>
    public class ProductTypeGet
    {
        /// <summary>
        ///     The product-types' ID.
        /// </summary>
        [Required]
        public int Id { get; }

        /// <summary>
        ///     A collection that contains the translations for this product-type.
        /// </summary>
        [Required]
        public IEnumerable<Translation> Translations { get; }


        public ProductTypeGet(int id, IEnumerable<Translation> translations)
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
    }
}
