using Common.Enums;
using FeatureCatalog.Infrastructure.Models.Database.Joins;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Models.Database;
using ProductCatalog.Infrastructure.Models.Database.Joins;

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

                // Product (previously "Module")
                entity.HasData(new ProductType { Id = 2, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation { ProductTypeId = 2, Language = "en", Name = "Product", Description = "A base-module. These types of products are stand-alone product offerings, and is typically considered a 'base' or 'core' product.", UpdatedBy = systemUserId },
                    new ProductTypeTranslation { ProductTypeId = 2, Language = "nb", Name = "Produkt", Description = "En basis-modul. Dette er frittstående produkter, og er normalt ett basis- eller kjerne-produkt.", UpdatedBy = systemUserId },
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
                //TODO: Remove will not be in use when Implement is removed
                // Basic User Management 
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicUserManagement, AccessControlPermissionNode = "BasicUserManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, Language = "en", Name = "Basic User Management", Description = "Allows organizations to perform the basic user management tasks. This is an extension of the options that's available for all organizations.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, Language = "nb", Name = "Grunnleggende brukerhåndtering", Description = "Lar en organisasjon utføre grunnleggende brukerbehandling. Dette er en utvidelse av funksjonaliteten som er tilgjengelig for alle organisasjoner.", UpdatedBy = systemUserId }
                });
                //TODO: Will not be in use when Implement is removed
                // Basic Asset Management
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicAssetManagement, AccessControlPermissionNode = "BasicAssetManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, Language = "en", Name = "Basic Asset Management", Description = "Allows organizations to perform the basic user management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, Language = "nb", Name = "Grunnleggende asset-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av assets.", UpdatedBy = systemUserId },
                });
                //TODO: Remove as it is a old value (New value is Subscription management)
                // Basic Subscription Management
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicSubscriptionManagement, AccessControlPermissionNode = "BasicSubscriptionManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicSubscriptionManagement, Language = "en", Name = "Basic Subscription Management", Description = "Allows organizations to perform the basic subscription management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicSubscriptionManagement, Language = "nb", Name = "Grunnleggende abonnement-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av abonnementer.", UpdatedBy = systemUserId }
                });
                //TODO: Remove when frontend check for new values
                // Basic Non-personal Asset Management
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicNonPersonalAssetManagement, AccessControlPermissionNode = "BasicNonPersonalAssetManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicNonPersonalAssetManagement, Language = "en", Name = "Basic Non-personal Asset Management", Description = "Allows organizations to perform non-personal asset management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicNonPersonalAssetManagement, Language = "nb", Name = "Grunnleggende administrering av ikke-personlige assets", Description = "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", UpdatedBy = systemUserId },
                });
                //TODO: Remove as it is the old value for 
                // Basic Book Value Management
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicBookValueManagement, AccessControlPermissionNode = "BasicBookValueManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, Language = "en", Name = "Basic Book Value Management", Description = "Allows organizations to Book value and Purchase price related tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, Language = "nb", Name = "Håndtering av Bokført verdi", Description = "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", UpdatedBy = systemUserId },
                });
                //TODO: Remove as it is the old value (New value Internal Asset Return)
                // Basic transactional asset return
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, AccessControlPermissionNode = "BasicTransactionalAssetReturn", FeatureTypeId = (int)FeatureTypeSeedDataValues.AssetReturn, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, Language = "en", Name = "Basic asset return (transactional)", Description = "Makes it possible to use the 'return' functionality on supported transactional devices. This enables the most basic 'internal return' functionality, and is required when using/activating some of the more advanced return functionality.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, Language = "nb", Name = "Grunnleggende asset retur (transaksjonell)", Description = "Gjør det mulig å benytte 'returner' funksjonaliteten på støttede transaksjonelle enheter. Dette tilgjengeliggjør den mest grunnleggende 'intern retur' funksjonaliteten, og er nødvendig for å kunne bruke/aktivere noen av de mer avanserte retur funksjonalitetene.", UpdatedBy = systemUserId },
                });

                // Subscription Management - earlier -> (Basic Subscription management)
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.SubscriptionManagement, AccessControlPermissionNode = "SubscriptionManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.SubscriptionManagement, Language = "en", Name = "Subscription Management", Description = "Allows organizations to perform subscription management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.SubscriptionManagement, Language = "nb", Name = "Abonnent håndtering", Description = "Gir kunder tilgang til administrering av abonnementer.", UpdatedBy = systemUserId },
                });

                // Asset Book Value - earlier ->  (Basic Book Value Management)
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.AssetBookValue, AccessControlPermissionNode = "AssetBookValue", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.AssetBookValue, Language = "en", Name = "Asset Book Value", Description = "Allows organizations to book value and purchase price related tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.AssetBookValue, Language = "nb", Name = "Håndtering av bokført verdi", Description = "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", UpdatedBy = systemUserId },
                });

                // Internal Asset Return - earlier -> (Basic Transactional Asset Return)
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.InternalAssetReturn, AccessControlPermissionNode = "InternalAssetReturn", FeatureTypeId = (int)FeatureTypeSeedDataValues.AssetReturn, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.InternalAssetReturn, Language = "en", Name = "Internal Asset Return", Description = "The organization handles the return of assets internally.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.InternalAssetReturn, Language = "nb", Name = "Intern asset retur", Description = "Organisasjonen håndterer return av asset internt.", UpdatedBy = systemUserId },
                });

                // Recycle & Wipe Asset Return - earlier -> (RecycleAndWipeAssetReturn)
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, AccessControlPermissionNode = "RecycleAndWipeAssetReturn", FeatureTypeId = (int)FeatureTypeSeedDataValues.AssetReturn, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, Language = "en", Name = "Recycle & Wipe asset return", Description = "Makes it possible to return supported assets through 'Recycle & Wipe' service-orders.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, Language = "nb", Name = "Recycle & Wipe asset retur", Description = "Gjør det mulig å returnere støttede enheter via 'Recycle & Wipe' service-ordrer.", UpdatedBy = systemUserId },
                });
                // Basic Hardware Repair 
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BasicHardwareRepair, AccessControlPermissionNode = "BasicHardwareRepair", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId }); 
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicHardwareRepair, Language = "en", Name = "Basic Hardware Repair", Description = "Makes it possible for organizations to create ordinary repair/service-orders on supported hardware assets.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BasicHardwareRepair, Language = "nb", Name = "Grunnlegende Hardware Reparasjon", Description = "Gjør det mulig for organisasjoner å opprette ordinære reparasjon/service-ordrer på støttede hardware assets.", UpdatedBy = systemUserId },
                });

                // Employee Access 
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.EmployeeAccess, AccessControlPermissionNode = "EmployeeAccess", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.EmployeeAccess, Language = "en", Name = "Employee Access", Description = "Enables the various employee functionality/access.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.EmployeeAccess, Language = "nb", Name = "Ansatt brukertilgang", Description = "Muliggjør de forskjellige funksjonalitetene og adgang for ansatte.", UpdatedBy = systemUserId }
                });

                // Department Structure
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.DepartmentStructure, AccessControlPermissionNode = "DepartmentStructure", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId});
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.DepartmentStructure, Language = "en", Name = "Department Structure", Description = "Enables the various department functionality/access.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.DepartmentStructure, Language = "nb", Name = "Avdelings struktur", Description = "Muliggjør de forskjellige funksjonalitetene og adgang for avdelinger i selskapet.", UpdatedBy = systemUserId },
                });

                // Onboarding and Offboarding
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.OnAndOffboarding, AccessControlPermissionNode = "OnAndOffboarding", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.OnAndOffboarding, Language = "en", Name = "Onboarding and Offboarding", Description = "Enables functionality for on- and offboarding of user and customer.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.OnAndOffboarding, Language = "nb", Name = "Oppstart og avluttning", Description = "Muliggjør de forskjellige funksjonalitetene rund en bruker og/eller kundes oppstart eller avsluttning.", UpdatedBy = systemUserId }
                });

                // Buyout Asset 
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.BuyoutAsset, AccessControlPermissionNode = "BuyoutAsset", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId }); 
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BuyoutAsset, Language = "en", Name = "Buyout Asset", Description = "Makes it possible for users to buy out assets.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.BuyoutAsset, Language = "nb", Name = "Utkjøp av asset", Description = "Muliggjør oppkjøp av assets for brukere.", UpdatedBy = systemUserId },
                });

                // Asset Management 
                entity.HasData(new Feature { Id = (int)FeatureSeedDataValues.AssetManagement, AccessControlPermissionNode = "AssetManagement", FeatureTypeId = (int)FeatureTypeSeedDataValues.Unknown, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<FeatureTranslation>()
                {
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.AssetManagement, Language = "en", Name = "Asset Management", Description = "Allows organizations to perform asset management tasks.", UpdatedBy = systemUserId },
                    new FeatureTranslation { FeatureId = (int)FeatureSeedDataValues.AssetManagement, Language = "nb", Name = "Asset-håndtering", Description = "Lar en organisasjon utføre behandling av assets.", UpdatedBy = systemUserId },
                });
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
                entity.HasData(new Product { Id = (int)ProductSeedDataValues.SubscriptionManagement, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, Language = "en", Name = "Subscription management", Description = "A partner product based subscription management", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, Language = "nb", Name = "Abonnement-håndtering", Description = "Ett partner spesifikk abonnement-håndtering produkt", UpdatedBy = systemUserId },
                });

                //TODO: Remove Implement as a product
                // Implement
                entity.HasData(new Product { Id = (int)ProductSeedDataValues.Implement, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Module, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.Implement, Language = "en", Name = "Implement", Description = "Simple Asset Management for units purchased transactionally in Techstep's own WebShop.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.Implement, Language = "nb", Name = "Implement", Description = "Enkel Asset Management for enheter kjøpt transaksjonelt i Techstep egen nettbutikk.", UpdatedBy = systemUserId },
                });

                // Transactional Device Lifecycle
                entity.HasData(new Product { Id = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Module, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, Language = "en", Name = "Transactional Device Lifecycle Management", Description = "Lifecycle management for transactional devices.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, Language = "nb", Name = "Transactional Device Lifecycle Management", Description = "Livssyklusadministrasjon for transaksjonelle enheter", UpdatedBy = systemUserId },
                });

                // Book Value
                entity.HasData(new Product { Id = (int)ProductSeedDataValues.BookValue, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.BookValue, Language = "en", Name = "Book Value", Description = "Allow book value.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.BookValue, Language = "nb", Name = "Bokført verdi", Description = "Tilgjengeliggjør bokført verdi.", UpdatedBy = systemUserId },
                });

                // Recycle & Wipe
                entity.HasData(new Product { Id = (int)ProductSeedDataValues.RecycleAndWipe, PartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"), ProductTypeId = (int)ProductTypeSeedDataValue.Option, UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new List<ProductTranslation>()
                {
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, Language = "en", Name = "Recycle & Wipe", Description = "Allows 'Recycle & Wipe' to be selected as a aftermarket option in the organizations asset-category settings.", UpdatedBy = systemUserId },
                    new ProductTranslation { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, Language = "nb", Name = "Recycle & Wipe", Description = "Tilgjengeliggjør 'Recycle & Wipe' som ett alternativ inne i organisasjonens asset-kategori innstillinger.", UpdatedBy = systemUserId },
                });
            });


            /*
             * Adds features (functionality) to products.
             */
            modelBuilder.Entity<ProductFeature>(entity =>
            {
                // Add features to: Subscription management
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, FeatureId = (int)FeatureSeedDataValues.SubscriptionManagement, UpdatedBy = systemUserId });
                //TODO: Should be removed when frontend checks for the right feature
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, FeatureId = (int)FeatureSeedDataValues.BasicSubscriptionManagement, UpdatedBy = systemUserId });

                //TODO: Remove the feature/product mapping for Implement 
                // Add features to: Implement
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.Implement, FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.Implement, FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, UpdatedBy = systemUserId });

                // Add features to: Transactional Device Lifecycle
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.EmployeeAccess, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.DepartmentStructure, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.OnAndOffboarding, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.AssetManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BuyoutAsset, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicHardwareRepair, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.InternalAssetReturn, UpdatedBy = systemUserId });

                //TODO: Remove when frontend check for the right feature
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicUserManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicAssetManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicNonPersonalAssetManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, UpdatedBy = systemUserId });

                // Add features to: Book Value
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.BookValue, FeatureId = (int)FeatureSeedDataValues.AssetBookValue, UpdatedBy = systemUserId });
                //TODO: Remove when frontend checks for the right permissions
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.BookValue, FeatureId = (int)FeatureSeedDataValues.BasicBookValueManagement, UpdatedBy = systemUserId });


                // Add features to: Recycle & Wipe
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, FeatureId = (int)FeatureSeedDataValues.RecycleAndWipeAssetReturn, UpdatedBy = systemUserId });
                //TODO: Remove when the frontend check for the right permissions
                entity.HasData(new ProductFeature { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, FeatureId = (int)FeatureSeedDataValues.BasicTransactionalAssetReturn, UpdatedBy = systemUserId });

            });


            /*
             * Defines the 'Require One' dependencies for products.
             */
            modelBuilder.Entity<ProductRequiresOne>(entity =>
            {
                //TODO: Should be removed when the Implement is removed and is the default product for all customers 
                //Implement 
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, RequiresProductId = (int)ProductSeedDataValues.Implement, UpdatedBy = systemUserId });
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.BookValue, RequiresProductId = (int)ProductSeedDataValues.Implement, UpdatedBy = systemUserId });

                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.RecycleAndWipe, RequiresProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.SubscriptionManagement, RequiresProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, UpdatedBy = systemUserId });
                entity.HasData(new ProductRequiresOne { ProductId = (int)ProductSeedDataValues.BookValue, RequiresProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, UpdatedBy = systemUserId });
            });

            /*
           * Defines the 'Excludes' dependencies for products.
           */
            modelBuilder.Entity<ProductExcludes>(entity =>
            {
                //TODO: This should be removed when implement is removed and has become the default product for all customers
                entity.HasData(new ProductExcludes { ProductId = (int)ProductSeedDataValues.Implement, ExcludesProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, UpdatedBy = systemUserId }); // Implement module is exclusive of Transactional Module
                entity.HasData(new ProductExcludes { ProductId = (int)ProductSeedDataValues.TransactionalDeviceLifecycleManagement, ExcludesProductId = (int)ProductSeedDataValues.Implement, UpdatedBy = systemUserId }); // Transactional module is exclusive of Implement module
            });

            #endregion Products

#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
