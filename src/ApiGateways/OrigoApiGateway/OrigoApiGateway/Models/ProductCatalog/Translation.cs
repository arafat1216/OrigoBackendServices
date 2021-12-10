using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.ProductCatalog
{
    /// <summary>
    ///     Represents a single translation entry.
    /// </summary>
    public class Translation
    {
        /// <summary>
        ///     The language used for this translation. <para>
        ///     
        ///     For requests where no corresponding translations was found, the returned translation may fall back to <c>en</c> to ensure a value is provided. </para> 
        /// </summary>
        [Required]
        [RegularExpression("^[a-z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
        public string Language { get; set; }

        /// <summary>
        ///     The name in the requested <see cref="Language"/>. If the <see cref="Language"/> is not defined or don't exist, this will be in English (<c>en</c>).
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        ///     A detailed description in the requested <see cref="Language"/>. If the <see cref="Language"/> is not defined or don't exist,
        ///     this will be in English (<c>en</c>).
        /// </summary>
        public string? Description { get; set; }
    }
}
