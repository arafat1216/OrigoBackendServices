using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context.EntityConfiguration;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context
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
        }
    }
}
