using ProductCatalog.Domain.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Products
{
    public class ProductTypeUpdate : ProductTypePost
    {
        public ProductTypeUpdate(int id, List<Translation> translations) : base(translations)
        {
            Id = id;
        }

        [Required]
        public int Id { get; }
    }
}
