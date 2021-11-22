using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    public class FeatureType : Entity
    {
        public int Id { get; set; }
        public virtual ICollection<FeatureTypeTranslation> Translations { get; set; } = new HashSet<FeatureTypeTranslation>();

        // EF Navigation
        public virtual IReadOnlyCollection<Feature> Features { get; set; } = new HashSet<Feature>();
    }
}
