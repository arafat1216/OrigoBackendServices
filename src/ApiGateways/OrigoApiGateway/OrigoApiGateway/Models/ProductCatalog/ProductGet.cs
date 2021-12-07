using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models.ProductCatalog
{
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
        public IEnumerable<Translation> Translations { get; }

        public Requirement Requirements { get; }


        public ProductGet()
        {
            Translations = new List<Translation>();
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
