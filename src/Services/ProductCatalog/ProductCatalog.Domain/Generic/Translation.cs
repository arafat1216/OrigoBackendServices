using ProductCatalog.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Generic
{
    public class Translation : ITranslationResult
    {
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        public string Language { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string? Description { get; set; }


        public Translation(string language, string name, string? description)
        {
            Language = language;
            Name = name;
            Description = description;
        }
    }
}
