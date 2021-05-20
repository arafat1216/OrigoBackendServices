﻿using AssetServices.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Infrastructure
{
    public class AssetsContext : DbContext
    {
        public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>().ToTable("Asset");
            modelBuilder.Entity<Asset>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}