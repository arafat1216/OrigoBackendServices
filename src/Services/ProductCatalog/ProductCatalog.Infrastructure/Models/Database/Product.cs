using ProductCatalog.Common.Interfaces;
using ProductCatalog.Infrastructure.Models.Database.Joins;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Represents a single partner-specific product that is sold/offered to customers.
    /// </summary>
    internal class Product : Entity, ITranslatable<ProductTranslation>
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The database-generated primary key.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        ///     The partner that owns and sells this product.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        ///     A foreign-key to the corresponding <see cref="ProductType.Id"/> that is used for this product.
        /// </summary>
        public int ProductTypeId { get; set; }

        /*
         * EF Owned Tables
         */

        /// <summary>
        ///     Contains the internationalization (i18n) translations.
        /// </summary>
        public virtual ICollection<ProductTranslation> Translations { get; set; }

        /*
         * EF Navigation
         */

        public virtual ProductType? ProductType { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<Feature> Features { get; set; } = new HashSet<Feature>();


        public virtual ICollection<ProductExcludes> Excludes { get; set; } = new HashSet<ProductExcludes>();
        public virtual IEnumerable<int> ExcludesAsIds { get { return Excludes.Select(e => e.ExcludesProductId); } }

        public virtual ICollection<ProductRequiresAll> RequiresAll { get; set; } = new HashSet<ProductRequiresAll>();
        public virtual IEnumerable<int> RequiresAllAsIds { get { return RequiresAll.Select(e => e.RequiresProductId); } }

        public virtual ICollection<ProductRequiresOne> RequiresOne { get; set; } = new HashSet<ProductRequiresOne>();
        public virtual IEnumerable<int> RequiresOneAsIds { get { return RequiresOne.Select(e => e.RequiresProductId); } }


        [NotMapped]
        public virtual ICollection<ProductExcludes> HasExcludesDependenciesFrom { get; set; } = new HashSet<ProductExcludes>();
        //public virtual ICollection<Product> HasExcludesDependenciesFrom { get; set; } = new HashSet<Product>();

        [NotMapped]
        public virtual ICollection<ProductRequiresAll> HasRequiresAllDependenciesFrom { get; set; } = new HashSet<ProductRequiresAll>();
        //public virtual ICollection<Product> HasRequiresAllDependenciesFrom { get; set; } = new HashSet<Product>();

        [NotMapped]
        public virtual ICollection<ProductRequiresOne> HasRequiresOneDependenciesFrom { get; set; } = new HashSet<ProductRequiresOne>();
        //public virtual ICollection<Product> HasRequiresOneDependenciesFrom { get; set; } = new HashSet<Product>();


        /*
         * EF Join Tables
         */

        internal virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        public Product() : base()
        {
            Translations = new HashSet<ProductTranslation>();
        }


        public Product(Guid partnerId, int productTypeId, ICollection<ProductTranslation> translations, Guid updatedBy) : base(updatedBy)
        {
            PartnerId = partnerId;
            ProductTypeId = productTypeId;
            Translations = translations;
        }
    }
}
