using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Models.Database.Joins;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Infrastructure.Models.Database
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


        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Feature() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        public Feature(int featureTypeId, string accessControlPermissionNode, ICollection<FeatureTranslation> translations, Guid updatedBy) : base(updatedBy)
        {
            FeatureTypeId = featureTypeId;
            AccessControlPermissionNode = accessControlPermissionNode;
            Translations = translations;
        }
    }
}
