using ProductCatalog.Domain.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.ProductTypes
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


        public ProductTypePost(List<Translation> translations, Guid updatedBy)
        {
            Translations = translations;
            UpdatedBy = updatedBy;
        }
    }
}
