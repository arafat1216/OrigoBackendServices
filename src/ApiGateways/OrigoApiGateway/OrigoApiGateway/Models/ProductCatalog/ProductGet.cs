using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models.ProductCatalog
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
        public int Id { get; init; }

        /// <summary>
        ///     The partner the product is associated with.
        /// </summary>
        [Required]
        public Guid PartnerId { get; init; }

        /// <summary>
        ///     The ID for the product's current product-type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ProductTypeId { get; init; }

        /// <summary>
        ///     The product's current product-type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ProductTypeGet? ProductType { get; init; }

        /// <summary>
        ///     A collection that contains the translations for this product.
        /// </summary>
        [Required]
        public IEnumerable<Translation> Translations { get; init; } 

        /// <summary>
        ///     Contains requirements for the product.
        /// </summary>
        public Requirement Requirements { get; init; } 


        public ProductGet()
        {
            // Do not place default constructors in here. They should be injected during the HTTP deserialization process.
        }


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
