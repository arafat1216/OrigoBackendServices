using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    public class Feature : Entity
    {
        public int Id { get; set; }
        public int FeatureTypeId { get; set; }
        public string AccessControlPermissionNode { get; set; }

        // EF Navigation
        public virtual ICollection<FeatureTranslation> Translations { get; set; } = new HashSet<FeatureTranslation>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual FeatureType Type { get; set; }

        public virtual ICollection<Feature> Excludes { get; set; } = new HashSet<Feature>();
        public virtual ICollection<Feature> RequiresAll { get; set; } = new HashSet<Feature>();
        public virtual ICollection<Feature> RequiresOne { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasExcludesDependencyFrom { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasRequiresAllDependencyFrom { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasRequiresOneDependencyFrom { get; set; } = new HashSet<Feature>();

        // EF "Shadow navigation"
        internal virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();
    }
}
