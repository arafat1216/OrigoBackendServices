using System;
using System.Collections.Generic;
using System.Text.Json;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerContext : DbContext
    {
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<User> Users => Set<User>();
        public DbSet<AssetCategoryType> AssetCategoryTypes => Set<AssetCategoryType>();
        public DbSet<AssetCategoryLifecycleType> AssetCategoryLifecycleTypes => Set<AssetCategoryLifecycleType>();
        public DbSet<Permission> Permissions => Set<Permission>();

        public DbSet<PermissionSet> PermissionSets => Set<PermissionSet>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserPermissions> UserPermissions => Set<UserPermissions>();
        public DbSet<OrganizationPreferences> OrganizationPreferences => Set<OrganizationPreferences>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Partner> Partners => Set<Partner>();
        public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();


        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<AssetCategoryLifecycleType>().ToTable("AssetCategoryLifecycleType");
            modelBuilder.Entity<AssetCategoryType>().ToTable("AssetCategory");
            modelBuilder.Entity<Partner>().ToTable("Partner");

            modelBuilder.Entity<UserPermissions>().Property(userPermissions => userPermissions.AccessList)
                .HasConversion(
                    convertTo =>
                        JsonSerializer.Serialize(convertTo, new JsonSerializerOptions { IgnoreNullValues = true }),
                    convertFrom => JsonSerializer.Deserialize<IList<Guid>>(convertFrom,
                        new JsonSerializerOptions { IgnoreNullValues = true }));

            modelBuilder.Seed();
        }
    }
}