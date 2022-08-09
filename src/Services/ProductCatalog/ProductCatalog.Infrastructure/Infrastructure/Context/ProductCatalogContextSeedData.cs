using FeatureCatalog.Infrastructure.Models.Database.Joins;
using ProductCatalog.Infrastructure.Models.Database.Joins;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Models.Database;
using System.Xml.Linq;

namespace ProductCatalog.Infrastructure.Infrastructure.Context
{
    /// <summary>
    ///     Partial class that extends <see cref="ProductCatalogContext"/> with all seeding related methods. 
    ///     This is done to provide a better overview and separation of the contents.
    /// </summary>
    internal partial class ProductCatalogContext
    {

        /// <summary>
        ///     Seed the database with required production entries.
        /// </summary>
        /// <param name="modelBuilder"> The <see cref="ModelBuilder"/> to use. </param>
        private void SeedProductionData(ModelBuilder modelBuilder)
        {
#pragma warning disable CS0618 // Type or member is obsolete

            Guid systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");


            /*
             * Defines the feature-types and their translations.
             */
            modelBuilder.Entity<FeatureType>(entity =>
            {
                // Unknown
                entity.HasData(new FeatureType { Id = 1, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTypeTranslation>()
                {
                    new FeatureTypeTranslation { FeatureTypeId = 1, Language = "en", Name = "Unknown", Description = "The type is not set, or is a invalid value!", UpdatedBy = systemUserId },
                    new FeatureTypeTranslation { FeatureTypeId = 1, Language = "nb", Name = "Ukjent", Description = "Typen er ikke satt, eller er en ugyldig verdi!", UpdatedBy = systemUserId },
                });

                // Asset return
                entity.HasData(new FeatureType { Id = 2, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTypeTranslation>()
{
                    new FeatureTypeTranslation { FeatureTypeId = 2, Language = "en", Name = "Asset return", Description = "This type contains functionality that enables return, recycle and other similar functionality for assets.", UpdatedBy = systemUserId },
                    new FeatureTypeTranslation { FeatureTypeId = 2, Language = "nb", Name = "Asset retur", Description = "Denne typen inneholder funksjoner som muliggjør retur, resirkulering og andre lignende funksjonaliteter på assets.", UpdatedBy = systemUserId },
                });
            });


            /*
             * Defines the product-types and their translations.
             */
            modelBuilder.Entity<ProductType>(entity =>
            {
                // Unknown
                entity.HasData(new ProductType { Id = 1, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation { ProductTypeId = 1, Language = "en", Name = "Unknown", Description = "The type is not set, or is a invalid value!", UpdatedBy = systemUserId },
                    new ProductTypeTranslation { ProductTypeId = 1, Language = "nb", Name = "Ukjent", Description = "Typen er ikke satt, eller er en ugyldig verdi!", UpdatedBy = systemUserId },
                });

                // Module
                entity.HasData(new ProductType { Id = 2, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation { ProductTypeId = 2, Language = "en", Name = "Module", Description = "A base-module. These types of products are stand-alone product offerings, and is typically considered a 'base' or 'core' product.", UpdatedBy = systemUserId },
                    new ProductTypeTranslation { ProductTypeId = 2, Language = "nb", Name = "Modul", Description = "En basis-modul. Dette er frittstående produkter, og er normalt ett basis- eller kjerne-produkt.", UpdatedBy = systemUserId },
                });

                // Options
                entity.HasData(new ProductType { Id = 3, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation { ProductTypeId = 3, Language = "en", Name = "Option", Description = "An option is a addon-product enables extra functionality in one or more of the modules.", UpdatedBy = systemUserId },
                    new ProductTypeTranslation { ProductTypeId = 3, Language = "nb", Name = "Opsjon", Description = "En opsjon er ett tillegsprodukt som aktiverer ekstra funksjonalitet i en eller flere moduler.", UpdatedBy = systemUserId },
                });
            });


            #region Features

            /*
             * Defines the features (permission nodes) that is possible to include in products, along with their translations.
             */
            modelBuilder.Entity<Feature>(entity =>
            {
                // Basic User Management
                entity.HasData(new Feature { Id = 1, AccessControlPermissionNode = "BasicUserManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 1, Language = "en", Name = "Basic User Management", Description = "Allows organizations to perform the basic user management tasks. This is an extension of the options that's available for all organizations.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 1, Language = "nb", Name = "Grunnleggende brukerhåndtering", Description = "Lar en organisasjon utføre grunnleggende brukerbehandling. Dette er en utvidelse av funksjonaliteten som er tilgjengelig for alle organisasjoner.", UpdatedBy = systemUserId }
                });

                // Basic Asset Management
                entity.HasData(new Feature { Id = 2, AccessControlPermissionNode = "BasicAssetManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 2, Language = "en", Name = "Basic Asset Management", Description = "Allows organizations to perform the basic user management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 2, Language = "nb", Name = "Grunnleggende asset-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av assets.", UpdatedBy = systemUserId },
                });

                // Basic Subscription Management
                entity.HasData(new Feature { Id = 3, AccessControlPermissionNode = "BasicSubscriptionManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 3, Language = "en", Name = "Basic Subscription Management", Description = "Allows organizations to perform the basic subscription management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 3, Language = "nb", Name = "Grunnleggende abonnement-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av abonnementer.", UpdatedBy = systemUserId }
                });

                // Basic Non-personal Asset Management
                entity.HasData(new Feature { Id = 4, AccessControlPermissionNode = "BasicNonPersonalAssetManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 4, Language = "en", Name = "Basic Non-personal Asset Management", Description = "Allows organizations to perform non-personal asset management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 4, Language = "nb", Name = "Grunnleggende administrering av ikke-personlige assets", Description = "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", UpdatedBy = systemUserId },
                });

                // Basic Book Value Management
                entity.HasData(new Feature { Id = 5, AccessControlPermissionNode = "BasicBookValueManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 5, Language = "en", Name = "Basic Book Value Management", Description = "Allows organizations to Book value and Purchase price related tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 5, Language = "nb", Name = "Håndtering av Bokført verdi", Description = "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", UpdatedBy = systemUserId },
                });

                // Basic transactional return
                entity.HasData(new Feature { Id = 6, AccessControlPermissionNode = "BasicTransactionalAssetReturn", FeatureTypeId = (int)FeatureTypeSeedDataValues.AssetReturn, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 6, Language = "en", Name = "Basic asset return (transactional)", Description = "Makes it possible to use the 'return' functionality on supported transactional devices. This enables the most basic 'internal return' functionality, and is required when using/activating some of the more advanced return functionality.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 6, Language = "nb", Name = "Grunnleggende asset retur (transaksjonell)", Description = "Gjør det mulig å benytte 'returner' funksjonaliteten på støttede transaksjonelle enheter. Dette tilgjengeliggjør den mest grunnleggende 'intern retur' funksjonaliteten, og er nødvendig for å kunne bruke/aktivere noen av de mer avanserte retur funksjonalitetene.", UpdatedBy = systemUserId },
                });

                // Recycle & Wipe return
                entity.HasData(new Feature { Id = 7, AccessControlPermissionNode = "RecycleAndWipeAssetReturn", FeatureTypeId = (int)FeatureTypeSeedDataValues.AssetReturn, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 7, Language = "en", Name = "Recycle & Wipe asset return", Description = "Makes it possible to return supported assets through 'Recycle & Wipe' service-orders.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 7, Language = "nb", Name = "Recycle & Wipe asset retur", Description = "Gjør det mulig å returnere støttede enheter via 'Recycle & Wipe' service-ordrer.", UpdatedBy = systemUserId },
                });

                // Hardware Repair
                entity.HasData(new Feature { Id = 8, AccessControlPermissionNode = "BasicHardwareRepair", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = 8, Language = "en", Name = "Basic Hardware Repair", Description = "Makes it possible for organizations to create ordinary repair/service-orders on supported hardware assets.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = 8, Language = "nb", Name = "Grunnlegende Hardware Reparasjon", Description = "Gjør det mulig for organisasjoner å opprette ordinære reparasjon/service-ordrer på støttede hardware assets.", UpdatedBy = systemUserId },
                });
            });


            /*
             * Defines the 'Require One' dependencies for features.
             */
            modelBuilder.Entity<FeatureRequiresOne>(entity =>
            {
                entity.HasData(new FeatureRequiresOne { FeatureId = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, RequiresFeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, UpdatedBy = systemUserId });
            });


            #endregion Features

            #region Products


            /*
             * Defines basic products, along with their translations.
             */
            modelBuilder.Entity<Product>(entity =>
            {
                // NB!
                //
                // The Partner ID '5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C' is for the 'Techstep ASA' partner, as used in the dev, test and production environments.
                // This partner may not exist locally unless it's been explicitly created or updated!

                // Subscription management
                entity.HasData(new Product { Id = 1, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = 1, Language = "en", Name = "Subscription management", Description = "A partner product based subscription management", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = 1, Language = "nb", Name = "Abonnement-håndtering", Description = "Ett partner spesifikk abonnement-håndtering produkt", UpdatedBy = systemUserId },
                });

                // Entry
                entity.HasData(new Product { Id = 2, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Module, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = 2, Language = "en", Name = "Entry", Description = "Simple Asset Management for units purchased transactionally in Techstep's own WebShop.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = 2, Language = "nb", Name = "Entry", Description = "Enkel Asset Management for enheter kjøpt transaksjonelt i Techstep egen nettbutikk.", UpdatedBy = systemUserId },
                });

                // Transactional Device Lifecycle
                entity.HasData(new Product { Id = 3, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Module, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = 3, Language = "en", Name = "Transactional Device Lifecycle Management", Description = "Lifecycle management for transactional devices.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = 3, Language = "nb", Name = "Transactional Device Lifecycle Management", Description = "Livssyklusadministrasjon for transaksjonelle enheter", UpdatedBy = systemUserId },
                });

                // Book Value and Purchase Price
                entity.HasData(new Product { Id = 4, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = 4, Language = "en", Name = "Book Value and Purchase Price", Description = "Allow book value and Purchase Price.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = 4, Language = "nb", Name = "Bokført verdi og kjøpspris", Description = "Tilgjengeliggjør bokført verdi og kjøpspris.", UpdatedBy = systemUserId },
                });

                // Recycle & Wipe
                entity.HasData(new Product { Id = 5, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = 5, Language = "en", Name = "Recycle & Wipe", Description = "Allows 'Recycle & Wipe' to be selected as a aftermarket option in the organizations asset-category settings.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = 5, Language = "nb", Name = "Recycle & Wipe", Description = "Tilgjengeliggjør 'Recycle & Wipe' som ett alternativ inne i organisasjonens asset-kategori innstillinger.", UpdatedBy = systemUserId },
                });
            });


            /*
             * Adds features (functionality) to products.
             */
            modelBuilder.Entity<ProductFeature>(entity =>
            {
                // Add features to: Subscription management
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, FeatureId = (int)FeatureSeedDataValues.BasicSubscriptionManagement, UpdatedBy = systemUserId });

                // Add features to: Entry
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.Entry, FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.Entry, FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, UpdatedBy = systemUserId });

                // Add features to: Transactional Device Lifecycle
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicNonPersonalAssetManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicHardwareRepair, UpdatedBy = systemUserId });

                // Add features to: Book Value and Purchase Price
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.BookValueAndPurchasePrice, FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, UpdatedBy = systemUserId });

                // Add features to: Recycle & Wipe
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, FeatureId = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, UpdatedBy = systemUserId });
            });


            /*
             * Defines the 'Require One' dependencies for products.
             */
            modelBuilder.Entity<ProductRequiresOne>(entity =>
            {
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, RequiresProductId = (int)ProductSeedDataValues.Entry, UpdatedBy = systemUserId });
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.BookValueAndPurchasePrice, RequiresProductId = (int)ProductSeedDataValues.Entry, UpdatedBy = systemUserId });
            });


            /*
             * Defines the 'Excludes' dependencies for products.
             */
            modelBuilder.Entity<ProductExcludes>(entity =>
            {
                entity.HasData(new ProductExcludes { ProductId = (int)ProductSeedDataValues.Entry, ExcludesProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, UpdatedBy = systemUserId }); // Entry module is exclusive of Transactional Module
                entity.HasData(new ProductExcludes { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, ExcludesProductId = (int)ProductSeedDataValues.Entry, UpdatedBy = systemUserId }); // Transactional module is exclusive of Entry module
            });

            #endregion Products

#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
