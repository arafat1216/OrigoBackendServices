using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context.EntityConfiguration;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context
{
    internal partial class ProductCatalogContext : DbContext
    {
        public DbSet<Feature> Features { get; set; } = null!;
        //public DbSet<FeatureTranslation> FeatureTranslations { get; set; } = null!;

        public DbSet<FeatureType> FeatureTypes { get; set; } = null!;
        //public DbSet<FeatureTypeTranslation> FeatureTypeTranslations { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;
        //public DbSet<ProductTranslation> ProductTranslations { get; set; } = null!;

        public DbSet<ProductType> ProductTypes { get; set; } = null!;
        //public DbSet<ProductTypeTranslation> ProductTypeTranslations { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<ProductFeature> ProductFeatures { get; set; } = null!;


        public ProductCatalogContext(DbContextOptions<ProductCatalogContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Feature tables
            modelBuilder.ApplyConfiguration(new FeatureConfiguration());
            //modelBuilder.ApplyConfiguration(new FeatureTranslationConfiguration());

            // Feature type tables
            modelBuilder.ApplyConfiguration(new FeatureTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new FeatureTypeTranslationConfiguration());

            // Product tables
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            //modelBuilder.ApplyConfiguration(new ProductTranslationConfiguration());

            // Product type tables
            modelBuilder.ApplyConfiguration(new ProductTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new ProductTypeTranslationConfiguration());

            // Order tables
            modelBuilder.ApplyConfiguration(new OrderConfiguration());

            // Misc join tables
            modelBuilder.ApplyConfiguration(new ProductFeatureConfiguration());
        }
    }
}
