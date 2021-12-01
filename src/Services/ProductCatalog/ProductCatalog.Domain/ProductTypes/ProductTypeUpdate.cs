using ProductCatalog.Domain.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.ProductTypes
{
    public class ProductTypeUpdate : ProductTypePost
    {
        [Required]
        public int Id { get; }


        public ProductTypeUpdate(int id, List<Translation> translations, Guid updatedBy) : base(translations, updatedBy)
        {
            Id = id;
        }
    }
}
