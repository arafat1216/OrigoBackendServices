using CustomerServices.Infrastructure.Context.EntityConfiguration;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Context
{
    public class CustomerContext : DbContext
    {
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<User> Users => Set<User>();
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
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new PartnerConfiguration());
            modelBuilder.ApplyConfiguration(new UserPermissionsConfiguration());

            modelBuilder.Seed();
        }
    }
}