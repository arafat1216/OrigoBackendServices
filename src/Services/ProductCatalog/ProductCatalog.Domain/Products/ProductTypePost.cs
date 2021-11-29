using ProductCatalog.Domain.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Products
{
    public class ProductTypePost
    {
        public ProductTypePost(List<Translation> translations)
        {
            Translations = translations;
        }


        [Required]
        public List<Translation> Translations { get; }
    }
}
