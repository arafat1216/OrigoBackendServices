using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Models
{
    public class AssetCategoryTranslation : Entity
    {
        [StringLength(2, ErrorMessage = "ISO-634 Language code max length is 2")]
        public string Language { get; protected set; }

        public string Name { get; protected set; }

        public string Description { get; set; }
    }
}
