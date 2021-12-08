using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Domain.ProductTypes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductCatalog.Domain.Products
{
    /// <summary>
    ///     Represents a single, existing product.
    /// </summary>
    public class ProductGet
    {
        /// <summary>
        ///     The products' ID.
        /// </summary>
        [Required]
        public int Id { get; }

        /// <summary>
        ///     The partner the product is associated with.
        /// </summary>
        [Required]
        public Guid PartnerId { get; }

        /// <summary>
        ///     The ID for the product's current product-type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ProductTypeId { get; }

        /// <summary>
        ///     The product's current product-type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ProductTypeGet? ProductType { get; }

        /// <summary>
        ///     A collection that contains the translations for this product.
        /// </summary>
        [Required]
        public IEnumerable<ITranslationResult> Translations { get; }

        /// <summary>
        ///     Contains requirements for the product.
        /// </summary>
        public Requirement Requirements { get; }


        public ProductGet(int id, Guid partnerId, int? productTypeId, ProductTypeGet? productType, IEnumerable<Translation> translations, Requirement requirements)
        {
            Id = id;
            PartnerId = partnerId;
            ProductTypeId = productTypeId;
            ProductType = productType;
            Translations = translations;
            Requirements = requirements;
        }


        public ProductGet(int id, Guid partnerId, int? productTypeId, ProductTypeGet? productType, IEnumerable<Translation> translations, IEnumerable<int> excludes, IEnumerable<int> requiresAll, IEnumerable<int> requiresOne)
        {
            Id = id;
            PartnerId = partnerId;
            ProductTypeId = productTypeId;
            ProductType = productType;
            Translations = translations;
            Requirements = new Requirement(excludes, requiresAll, requiresOne);
        }

    }
}
