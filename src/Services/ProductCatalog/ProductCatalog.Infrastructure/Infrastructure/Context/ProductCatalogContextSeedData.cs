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
            Guid systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");


            modelBuilder.Entity<FeatureType>(entity =>
            {
                // Unknown
                entity.HasData(new FeatureType { Id = 1, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new FeatureTypeTranslation { FeatureTypeId = 1, Language = "en", Name = "Unknown", Description = "The type is not set, or is a invalid value!", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new FeatureTypeTranslation { FeatureTypeId = 1, Language = "nb", Name = "Ukjent", Description = "Typen er ikke satt, eller er en ugyldig verdi!", UpdatedBy = systemUserId });
            });


            modelBuilder.Entity<ProductType>(entity =>
            {
                // Unknown
                entity.HasData(new ProductType { Id = 1, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 1, Language = "en", Name = "Unknown", Description = "The type is not set, or is a invalid value!", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 1, Language = "nb", Name = "Ukjent", Description = "Typen er ikke satt, eller er en ugyldig verdi!", UpdatedBy = systemUserId });


                // Module
                entity.HasData(new ProductType { Id = 2, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 2, Language = "en", Name = "Module", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 2, Language = "nb", Name = "Modul", UpdatedBy = systemUserId });


                // Options
                entity.HasData(new ProductType { Id = 3, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 3, Language = "en", Name = "Option", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new ProductTypeTranslation { ProductTypeId = 3, Language = "nb", Name = "Opsjon", UpdatedBy = systemUserId });
            });

            

            modelBuilder.Entity<Feature>(entity =>
            {
                // Basic User Management
                entity.HasData(new Feature { Id = 1, AccessControlPermissionNode = "BasicUserManagement", FeatureTypeId = 1, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 1, Language = "en", Name = "Basic User Management", Description = "Allows organizations to perform the basic user management tasks. This is an extension of the options that's available for all organizations.", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 1, Language = "nb", Name = "Grunnleggende brukerhåndtering", Description = "Lar en organisasjon utføre grunnleggende brukerbehandling. Dette er en utvidelse av funksjonaliteten som er tilgjengelig for alle organisasjoner.", UpdatedBy = systemUserId });


                // Basic Asset Management
                entity.HasData(new Feature { Id = 2, AccessControlPermissionNode = "BasicAssetManagement", FeatureTypeId = 1, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 2, Language = "en", Name = "Basic Asset Management", Description = "Allows organizations to perform the basic user management tasks.", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 2, Language = "nb", Name = "Grunnleggende asset-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av assets.", UpdatedBy = systemUserId });

                // Basic Subscription Management
                entity.HasData(new Feature { Id = 3, AccessControlPermissionNode = "BasicSubscriptionManagement", FeatureTypeId = 1, UpdatedBy = systemUserId });

                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 3, Language = "en", Name = "Basic Subscription Management", Description = "Allows organizations to perform the basic subscription management tasks.", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new FeatureTranslation { FeatureId = 3, Language = "nb", Name = "Grunnleggende abonnement-håndtering", Description = "Lar en organisasjon utføre grunnleggende behandling av abonnomenter.", UpdatedBy = systemUserId });
            });

            modelBuilder.Entity<Product>(entity => 
            {
                entity.HasData(new Product { Id = 1, PartnerId = Guid.Empty, ProductTypeId = 3 });

                entity.OwnsMany(e => e.Translations).HasData(new ProductTranslation { ProductId = 1, Language = "en", Name = "Subscription management", Description = "A partner product based subscription management", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new ProductTranslation { ProductId = 1, Language = "nb", Name = "Abonnement-håndtering", Description = "Ett partner spesifikk abonnement-håndtering produkt", UpdatedBy = systemUserId });

                entity.HasData(new Product { Id = 2, PartnerId = Guid.Empty, ProductTypeId = 2 });

                entity.OwnsMany(e => e.Translations).HasData(new ProductTranslation { ProductId = 2, Language = "en", Name = "Entry", Description = "Simple Asset Management for units purchased transactionally in Techstep's own WebShop.", UpdatedBy = systemUserId });
                entity.OwnsMany(e => e.Translations).HasData(new ProductTranslation { ProductId = 2, Language = "nb", Name = "Entry", Description = "Enkel Asset Management for enheter kjøpt transaksjonelt i Techstep egen nettbutikk.", UpdatedBy = systemUserId });

            });
            modelBuilder.Entity<ProductFeature>(entity =>{

                entity.HasData(new ProductFeature { ProductId = 1, FeatureId = 3});
                entity.HasData(new ProductFeature { ProductId = 2, FeatureId = 1});
                entity.HasData(new ProductFeature { ProductId = 2, FeatureId = 2});
            });

            modelBuilder.Entity<ProductRequiresOne>(entity =>{
                entity.HasData(new ProductRequiresOne { ProductId = 1, RequiresProductId = 2});
            });
            

        }
    }
}
