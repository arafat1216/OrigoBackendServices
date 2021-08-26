using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<AssetCategoryType> AssetCategoryTypes { get; set; }

        public DbSet<AssetCategoryLifecycleType> AssetCategoryLifecycleTypes { get; set; }

        public DbSet<ProductModule> ProductModules { get; set; }

        public DbSet<ProductModuleGroup> ProductModuleGroups { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetCategoryLifecycleType>().ToTable("AssetCategoryLifecycleType");
            modelBuilder.Entity<AssetCategoryType>().ToTable("AssetCategory");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Customer>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductModule>().ToTable("ProductModule");
        }
    }
}