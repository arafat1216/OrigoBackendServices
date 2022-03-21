using CustomerServices.Infrastructure.Context.EntityConfiguration;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

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

        #region Save overrides (inject automated timestamps)

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        /// <inheritdoc/>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <inheritdoc/>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        ///     Automatically updates the create, update and delete timestamps for all entries that utilizes the <see cref="Common.Seedwork.Entity"/> class.
        /// </summary>
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                                        .Where(x => x.Entity is Common.Seedwork.Entity
                                            && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                entity.Property("LastUpdatedDate").CurrentValue = now;

                if (entity.State == EntityState.Added)
                    entity.Property("CreatedDate").CurrentValue = now;
            }
        }

        #endregion
    }
}