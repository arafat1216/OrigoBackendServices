using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration;
using ProductCatalog.Infrastructure.Models.Database;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context
{
    internal partial class ProductCatalogContext : DbContext
    {
        public DbSet<Feature> Features { get; set; } = null!;

        public DbSet<FeatureType> FeatureTypes { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<ProductType> ProductTypes { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<ProductFeature> ProductFeatures { get; set; } = null!;


        public ProductCatalogContext(DbContextOptions<ProductCatalogContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Feature tables
            modelBuilder.ApplyConfiguration(new FeatureConfiguration());

            // Feature type tables
            modelBuilder.ApplyConfiguration(new FeatureTypeConfiguration());

            // Product tables
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            // Product type tables
            modelBuilder.ApplyConfiguration(new ProductTypeConfiguration());

            // Order tables
            modelBuilder.ApplyConfiguration(new OrderConfiguration());

            // Misc join tables
            modelBuilder.ApplyConfiguration(new ProductFeatureConfiguration());

            modelBuilder.ApplyConfiguration(new ProductExcludesConfiguration());
            modelBuilder.ApplyConfiguration(new ProductRequiresOneConfiguration());
            modelBuilder.ApplyConfiguration(new ProductRequiresAllConfiguration());

            modelBuilder.ApplyConfiguration(new FeatureExcludesConfiguration());
            modelBuilder.ApplyConfiguration(new FeatureRequiresOneConfiguration());
            modelBuilder.ApplyConfiguration(new FeatureRequiresAllConfiguration());
        }
    }
}
