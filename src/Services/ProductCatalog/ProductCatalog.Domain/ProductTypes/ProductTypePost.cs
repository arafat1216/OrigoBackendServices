using ProductCatalog.Domain.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.ProductTypes
{
    public class ProductTypePost
    {
        [Required]
        public List<Translation> Translations { get; }

        [Required]
        public Guid UpdatedBy { get; set; }


        public ProductTypePost(List<Translation> translations, Guid updatedBy)
        {
            Translations = translations;
            UpdatedBy = updatedBy;
        }
    }
}
