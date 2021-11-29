using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    internal class Product : Entity, ITranslatable<ProductTranslation>
    {
        // EF DB Columns
        public int Id { get; private set; }
        public Guid PartnerId { get; set; }
        public int ProductTypeId { get; set; }

        // EF Owned Tables
        public virtual ICollection<ProductTranslation> Translations { get; set; } = new HashSet<ProductTranslation>();

        // EF Navigation
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();
               
        public virtual ICollection<Product> Excludes { get; set; } = new HashSet<Product>();
        public virtual ICollection<Product> RequiresAll { get; set; } = new HashSet<Product>();
        public virtual ICollection<Product> RequiresOne { get; set; } = new HashSet<Product>();

        [NotMapped]
        public virtual ICollection<Product> HasExcludesDependencyFrom { get; set; } = new HashSet<Product>();

        [NotMapped]
        public virtual ICollection<Product> HasRequiresAllDependencyFrom { get; set; } = new HashSet<Product>();

        [NotMapped]
        public virtual ICollection<Product> HasRequiresOneDependencyFrom { get; set; } = new HashSet<Product>();

        // EF Join Tables
        internal virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();
    }
}
