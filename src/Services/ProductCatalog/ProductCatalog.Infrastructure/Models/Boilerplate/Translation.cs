using ProductCatalog.Common.Interfaces;
using ProductCatalog.Infrastructure.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Infrastructure.Models.Boilerplate
{
    public class Translation : Entity, ITranslationResult
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


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Translation() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        public Translation(string language, string name, string? description, Guid updatedBy) : base(updatedBy)
        {
            Language = language;
            Name = name;
            Description = description;
        }
    }
}
