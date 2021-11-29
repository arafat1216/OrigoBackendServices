using ProductCatalog.Domain.Generic;
using ProductCatalog.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Products
{
    public class ProductGet
    {
        internal ProductGet(int id, Guid partnerId, int? productTypeId, ProductTypeGet? productType, IEnumerable<Translation> translations)
        {
            Id = id;
            PartnerId = partnerId;
            ProductTypeId = productTypeId;
            ProductType = productType;
            Translations = translations;
        }

        [Required]
        public int Id { get; }

        [Required]
        public Guid PartnerId { get; }

        public int? ProductTypeId { get; }

        public ProductTypeGet? ProductType { get; }

        [Required]
        public IEnumerable<ITranslationResult> Translations { get; }

    }
}
