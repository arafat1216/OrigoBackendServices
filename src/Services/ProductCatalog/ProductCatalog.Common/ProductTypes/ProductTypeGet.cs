using ProductCatalog.Common.Generic;
using ProductCatalog.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Common.ProductTypes
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


        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductTypeGet"/> class.
        /// </summary>
        /// <param name="id"> The product-types' ID. </param>
        /// <param name="translations"> A collection that contains the translations for this product-type. </param>
        public ProductTypeGet(int id, IEnumerable<Translation> translations)
        {
            Id = id;
            Translations = translations;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductTypeGet"/> class.
        /// </summary>
        /// <param name="id"> The product-types' ID. </param>
        /// <param name="translations"> A collection that contains the translations for this product-type. </param>
        public ProductTypeGet(int id, IEnumerable<ITranslationResult> translations)
        {
            var results = new TypeConverter().ToTranslation(translations);

            Id = id;
            Translations = results;
        }
    }
}
