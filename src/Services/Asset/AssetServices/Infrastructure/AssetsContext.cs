using AssetServices.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Infrastructure
{
    public class AssetsContext : DbContext
    {
        public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets {get; set;}
        public DbSet<AssetCategory> AssetCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobilePhone>().ToTable("MobilePhone");
            modelBuilder.Entity<Tablet>().ToTable("Tablet");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<HardwareSuperType>().ToTable("HardwareType");
            modelBuilder.Entity<SoftwareSuperType>().ToTable("SoftwareType");
            modelBuilder.Entity<AssetCategory>().ToTable("AssetCategory");
        }
    }
}