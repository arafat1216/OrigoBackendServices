using FeatureCatalog.Infrastructure.Models.Database.Joins;
using ProductCatalog.Common.Interfaces;
using ProductCatalog.Infrastructure.Models.Database.Joins;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Represents a single functionality that can be added to a Product. 
    ///     Each feature has its own unique <see cref="AccessControlPermissionNode">access-control</see> check.
    /// </summary>
    internal class Feature : Entity, ITranslatable<FeatureTranslation>
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     The database-generated primary key.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        ///     A foreign-key to the corresponding <see cref="FeatureType.Id"/> that is used for this feature.
        /// </summary>
        public int FeatureTypeId { get; set; }

        /// <summary>
        ///     A fixed and unique access-control node-name. This is used throughout the front- and back-end to enable or disable functionality for an organization,
        ///     based on whether or not they have access to the feature through their <see cref="Product">products</see>. <para>
        ///     
        ///     The node-name must consist of only characters, formated in PascalCase. Example: '<c>MyPermissionNodeIdentifier</c>'. </para>
        /// </summary>
        public string AccessControlPermissionNode { get; set; }


        /*
         * EF Owned Tables
         */

        /// <summary>
        ///     Contains the internationalization (i18n) translations.
        /// </summary>
        public virtual ICollection<FeatureTranslation> Translations { get; set; } = new HashSet<FeatureTranslation>();


        /*
         * EF Navigation
         */

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual FeatureType? Type { get; set; }

        public virtual ICollection<FeatureExcludes> Excludes { get; set; } = new HashSet<FeatureExcludes>();
        public virtual IEnumerable<int> ExcludesAsIds { get { return Excludes.Select(e => e.ExcludesFeatureId); } }

        public virtual ICollection<FeatureRequiresAll> RequiresAll { get; set; } = new HashSet<FeatureRequiresAll>();
        public virtual IEnumerable<int> RequiresAllAsIds { get { return RequiresAll.Select(e => e.RequiresFeatureId); } }

        public virtual ICollection<FeatureRequiresOne> RequiresOne { get; set; } = new HashSet<FeatureRequiresOne>();
        public virtual IEnumerable<int> RequiresOneAsIds { get { return RequiresOne.Select(e => e.RequiresFeatureId); } }

        [NotMapped]
        public virtual ICollection<FeatureExcludes> HasExcludesDependenciesFrom { get; set; } = new HashSet<FeatureExcludes>();

        [NotMapped]
        public virtual ICollection<FeatureRequiresAll> HasRequiresAllDependenciesFrom { get; set; } = new HashSet<FeatureRequiresAll>();

        [NotMapped]
        public virtual ICollection<FeatureRequiresOne> HasRequiresOneDependenciesFrom { get; set; } = new HashSet<FeatureRequiresOne>();

        /*
         * EF Join Tables
         */

        internal virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new HashSet<ProductFeature>();


        /*
         * Constructors
         */

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
