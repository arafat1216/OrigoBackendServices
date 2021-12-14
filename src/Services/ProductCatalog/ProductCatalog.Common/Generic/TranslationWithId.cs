using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Common.Generic
{
    /// <inheritdoc/>
    public class TranslationWithId : Translation
    {
        /// <summary>
        ///     The ID for the translation.
        /// </summary>
        [Required]
        public int Id { get; set; }


        public TranslationWithId(int id, string language, string name, string? description) : base(language, name, description)
        {
            Id = id;
        }
    }
}
