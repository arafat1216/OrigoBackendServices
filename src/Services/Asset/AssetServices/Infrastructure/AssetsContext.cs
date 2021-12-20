using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AssetServices.Infrastructure
{
    public class AssetsContext : DbContext
    {
        bool isSqlLite = false;
        public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
        {
            foreach (var extension in options.Extensions)
            {
                var typeName = extension.GetType().ToString();
                if (extension.GetType().ToString().Contains("Sqlite"))
                {
                    isSqlLite = true;
                }
            }
        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<HardwareAsset> HardwareAsset { get; set; }
        public DbSet<SoftwareAsset> SoftwareAsset { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<CustomerLabel> CustomerLabels { get; set; }
        public DbSet<AssetLabel> AssetLabels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>().ToTable("Asset");
            modelBuilder.Entity<Asset>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
            modelBuilder.Entity<MobilePhone>().ToTable("MobilePhone");
            modelBuilder.Entity<Tablet>().ToTable("Tablet");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<HardwareAsset>().ToTable("HardwareAsset");
            if (isSqlLite)
            {
                modelBuilder.Entity<HardwareAsset>().OwnsMany(h => h.Imeis, n =>
                {
                    n.Property("Id").ValueGeneratedNever();
                });
                modelBuilder.Entity<Asset>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("date('now')");
            }
            else
            {
                modelBuilder.Entity<HardwareAsset>().OwnsMany(h => h.Imeis);
            }
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
                b.HasData(new { Id = 2, AssetCategoryId = 1, Language = "NO", Name = "Mobiltelefon", Description = "Mobiltelefon", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
                b.HasData(new { Id = 3, AssetCategoryId = 2, Language = "EN", Name = "Tablet", Description = "Tablet", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
                b.HasData(new { Id = 4, AssetCategoryId = 2, Language = "NO", Name = "Nettbrett", Description = "Nettbrett", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, CreatedBy = Guid.NewGuid(), DeletedBy = Guid.Empty, IsDeleted = false, UpdatedBy = Guid.Empty });
            });

            modelBuilder.Entity<CustomerLabel>().ToTable("CustomerLabel");
            modelBuilder.Entity<AssetLabel>().ToTable("AssetLabel");

            modelBuilder.Entity<AssetLabel>()
            .HasKey(t => new { t.AssetId, t.LabelId });

            modelBuilder.Entity<AssetLabel>()
            .HasOne(pt => pt.Asset)
            .WithMany(p => p.AssetLabels)
            .HasForeignKey(pt => pt.AssetId);

            modelBuilder.Entity<AssetLabel>()
                .HasOne(pt => pt.Label)
                .WithMany(t => t.AssetLabels)
                .HasForeignKey(pt => pt.LabelId);
        }
    }
}