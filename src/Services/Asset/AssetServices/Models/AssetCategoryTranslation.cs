using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Models
{
    public class AssetCategoryTranslation : Entity
    {
        public AssetCategoryTranslation(int id, string language, string name, string description)
        {
            Id = id;
            Language = language;
            Name = name;
            Description = description;
        }
        [StringLength(2, ErrorMessage = "ISO-634 Language code max length is 2")]
        public string Language { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
