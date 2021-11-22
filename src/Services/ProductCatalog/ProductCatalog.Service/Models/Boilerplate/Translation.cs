using ProductCatalog.Service.Models.Database;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Service.Models.Boilerplate
{
    public class Translation : Entity, ITranslation
    {
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        public string Language { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string? Description { get ; set ; }
    }
}
