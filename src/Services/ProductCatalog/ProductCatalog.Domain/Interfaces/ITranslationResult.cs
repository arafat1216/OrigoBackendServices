using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Interfaces
{
    /// <summary>
    ///     Exposes a single translation results
    /// </summary>
    /// <seealso cref="ITranslatable{TTranslationEntity}"/>
    public interface ITranslationResult
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
