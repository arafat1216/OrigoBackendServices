using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Exposes a single translation results
    /// </summary>
    /// <seealso cref="ITranslatable{TTranslationEntity}"/>
    public interface ITranslationResult
    {
        /// <summary>
        ///     The language that is associated with the translation. This uses the <c>ISO 639-1</c> standard.
        /// </summary>
        /// <example> en </example>
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        string Language { get; set; }

        /// <summary>
        ///     The items name.
        /// </summary>
        [Required]
        [MaxLength(128)]
        string Name { get; set; }

        /// <summary>
        ///     An optional description.
        /// </summary>
        string? Description { get; set; }
    }
}
