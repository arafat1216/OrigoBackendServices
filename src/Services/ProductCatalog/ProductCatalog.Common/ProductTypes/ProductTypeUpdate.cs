using ProductCatalog.Common.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Common.ProductTypes
{
    public class ProductTypeUpdate : ProductTypePost
    {
        /// <summary>
        ///     The product-types' ID.
        /// </summary>
        [Required]
        public int Id { get; }


        public ProductTypeUpdate(int id, List<Translation> translations, Guid updatedBy) : base(translations, updatedBy)
        {
            Id = id;
        }
    }
}
