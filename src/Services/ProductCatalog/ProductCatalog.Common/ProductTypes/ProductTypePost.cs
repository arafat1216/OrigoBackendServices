using ProductCatalog.Common.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Common.ProductTypes
{
    /// <summary>
    ///     Represents a new product-type that should be created.
    /// </summary>
    public class ProductTypePost
    {
        /// <summary>
        ///     A list containing all translations for the new product-type. This <b>must</b> include the English ("en") translation.
        /// </summary>
        [Required]
        public List<Translation> Translations { get; }

        /// <summary>
        ///     The user that is creating the new product-type.
        /// </summary>
        [Required]
        public Guid UpdatedBy { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductTypePost"/> class.
        /// </summary>
        /// <param name="translations"> A list containing all translations for the new product-type. This <b>must</b> include the English ("en") translation. </param>
        /// <param name="updatedBy"> The user that is creating the new product-type. </param>
        public ProductTypePost(List<Translation> translations, Guid updatedBy)
        {
            Translations = translations;
            UpdatedBy = updatedBy;
        }
    }
}
