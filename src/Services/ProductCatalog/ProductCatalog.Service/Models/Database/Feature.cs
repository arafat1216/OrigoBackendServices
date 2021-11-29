using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    internal class Feature : Entity, ITranslatable<FeatureTranslation>
    {
        // EF DB Columns
        public int Id { get; private set; }
        public int FeatureTypeId { get; set; }
        public string AccessControlPermissionNode { get; set; }

        // EF Owned Tables
        public virtual ICollection<FeatureTranslation> Translations { get; set; } = new HashSet<FeatureTranslation>();

        // EF Navigation
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual FeatureType? Type { get; set; }

        public virtual ICollection<Feature> Excludes { get; set; } = new HashSet<Feature>();
        public virtual ICollection<Feature> RequiresAll { get; set; } = new HashSet<Feature>();
        public virtual ICollection<Feature> RequiresOne { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasExcludesDependencyFrom { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasRequiresAllDependencyFrom { get; set; } = new HashSet<Feature>();

        [NotMapped]
        public virtual ICollection<Feature> HasRequiresOneDependencyFrom { get; set; } = new HashSet<Feature>();

        // EF Join Tables
        internal virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();
    }
}
