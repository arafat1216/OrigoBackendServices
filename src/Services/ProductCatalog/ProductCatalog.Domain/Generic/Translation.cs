using ProductCatalog.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Generic
{
    /// <summary>
    ///     Represents a single translation entry.
    /// </summary>
    public class Translation : ITranslationResult
    {
        /// <summary>
        ///     The language for this translation. This must be in the <code>ISO 639-1</code> standard.
        /// </summary>
        /// <example>en</example>
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        public string Language { get; set; }

        /// <summary>
        ///     The translated name.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        ///     An optional translated description.
        /// </summary>
        public string? Description { get; set; }


        public Translation(string language, string name, string? description)
        {
            Language = language;
            Name = name;
            Description = description;
        }
    }
}
