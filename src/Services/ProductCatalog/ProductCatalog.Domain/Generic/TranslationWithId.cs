using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Generic
{
    public class TranslationWithId : Translation
    {
        [Required]
        public int Id { get; set; }


        public TranslationWithId(int id, string language, string name, string? description) : base(language, name, description)
        {
            Id = id;
        }
    }
}
