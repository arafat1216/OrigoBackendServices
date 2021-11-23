using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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
            modelBuilder.Entity<AssetCategory>().HasMany(p => p.Translations);

            modelBuilder.Entity<AssetCategory>(b =>
            {
                b.HasData(new { Id = 1, CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
                b.HasData(new { Id = 2, CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
            });

            modelBuilder.Entity<AssetCategoryTranslation>(b =>
            {
                b.HasData(new { Id = 1, AssetCategoryId = 1, Language = "EN", Name = "Mobile phone", Description = "Mobile phone", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
                b.HasData(new { Id = 2, AssetCategoryId = 2, Language = "EN", Name = "Tablet", Description = "Tablet", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
            });
        }
    }
}