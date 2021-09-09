using System;
using System.Collections.Generic;
using System.Text.Json;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<AssetCategoryType> AssetCategoryTypes { get; set; }

        public DbSet<AssetCategoryLifecycleType> AssetCategoryLifecycleTypes { get; set; }

        public DbSet<ProductModule> ProductModules { get; set; }

        public DbSet<ProductModuleGroup> ProductModuleGroups { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<PermissionSet> PermissionSets { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserPermissions> UserPermissions { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());

            modelBuilder.Entity<AssetCategoryLifecycleType>().ToTable("AssetCategoryLifecycleType");
            modelBuilder.Entity<AssetCategoryType>().ToTable("AssetCategory");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductModule>().ToTable("ProductModule");
            modelBuilder.Entity<UserPermissions>().Property(userPermissions => userPermissions.AccessList)
                .HasConversion(convertTo => JsonSerializer.Serialize(convertTo, new JsonSerializerOptions{IgnoreNullValues = true}),
                    convertFrom => JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(convertFrom, new JsonSerializerOptions{ IgnoreNullValues = true }));

            modelBuilder.Seed();
        }
    }
}