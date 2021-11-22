using AssetServices.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Infrastructure
{
    public class AssetsContext : DbContext
    {
        public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<HardwareAsset> HardwareAsset { get; set; }
        public DbSet<SoftwareAsset> SoftwareAsset { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>().ToTable("Asset");
            modelBuilder.Entity<Asset>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<MobilePhone>().ToTable("MobilePhone");
            modelBuilder.Entity<Tablet>().ToTable("Tablet");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<HardwareAsset>().ToTable("HardwareAsset");
            modelBuilder.Entity<HardwareAsset>().OwnsMany(h => h.Imeis);
            modelBuilder.Entity<SoftwareAsset>().ToTable("SoftwareAsset");
            modelBuilder.Entity<AssetCategory>().ToTable("AssetCategory");
        }
    }
}