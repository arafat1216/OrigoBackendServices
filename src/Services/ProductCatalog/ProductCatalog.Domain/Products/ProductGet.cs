using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Domain.ProductTypes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductCatalog.Domain.Products
{
    /// <summary>
    /// test
    /// </summary>
    public class ProductGet
    {
        [Required]
        public int Id { get; }

        [Required]
        public Guid PartnerId { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ProductTypeId { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ProductTypeGet? ProductType { get; }

        [Required]
        public IEnumerable<ITranslationResult> Translations { get; }

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
