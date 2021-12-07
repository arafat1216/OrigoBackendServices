using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.ProductCatalog
{
    public class ProductTypeGet
    {
        [Required]
        public int Id { get; }

        [Required]
        public IEnumerable<Translation> Translations { get; }
    }
}
