using CustomerServices.Infrastructure.Context.EntityConfiguration;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Context
{
    public class CustomerContext : DbContext
    {
        private readonly bool _isSQLite;

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
            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite"))
                    continue;

                _isSQLite = true;
                break;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new UserConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new PartnerConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new UserPermissionsConfiguration(_isSQLite));

            modelBuilder.Seed();
        }
    }
}