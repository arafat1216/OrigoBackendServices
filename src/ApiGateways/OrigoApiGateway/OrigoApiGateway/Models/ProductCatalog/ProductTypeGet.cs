using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.ProductCatalog
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
        public int Id { get; init; }

        /// <summary>
        ///     A collection that contains the translations for this product-type.
        /// </summary>
        [Required]
        public IEnumerable<Translation> Translations { get; init; }
    }
}
