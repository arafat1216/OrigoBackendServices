using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    internal class FeatureType : Entity, ITranslatable<FeatureTypeTranslation>
    {
        // EF DB Columns
        public int Id { get; private set; }

        // EF Owned Tables
        public virtual ICollection<FeatureTypeTranslation> Translations { get; set; } = new HashSet<FeatureTypeTranslation>();

        // EF Navigation
        public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();
    }
}
