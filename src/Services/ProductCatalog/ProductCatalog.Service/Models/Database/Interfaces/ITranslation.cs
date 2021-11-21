using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Service.Models.Database.Interfaces
{
    public interface ITranslation
    {
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        string Language { get; set; }

        [Required]
        [MaxLength(128)]
        string Name { get; set; } 

        string? Description { get; set; }
    }
}
