using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure.EntityConfiguration;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementContext : DbContext
    {
        bool isSqlLite = false;
        public SubscriptionManagementContext(DbContextOptions<SubscriptionManagementContext> options) : base(options)
        {
            foreach (var extension in options.Extensions)
            {
                var typeName = extension.GetType().ToString();
                if (extension.GetType().ToString().Contains("Sqlite"))
                {
                    isSqlLite = true;
                    break;
                }
            }
        }

        public DbSet<Operator> Operators { get; set; }
        public DbSet<CustomerOperatorAccount> CustomerOperatorAccounts { get; set; }
        public DbSet<SubscriptionProduct> SubscriptionProducts { get; set; }
        public DbSet<SubscriptionOrder> SubscriptionOrders { get; set; }
        public DbSet<Datapackage> Datapackages { get; set; }
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<CustomerOperatorSettings> CustomerOperatorSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guid createdById = Guid.Parse("00000000-0000-0000-0000-000000000000");



            modelBuilder.Entity<Operator>(entity =>
            {

                entity.HasData(new { Id = 1, OperatorName = "Telia - NO", Country = "nb", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
                entity.HasData(new { Id = 2, OperatorName = "Telia - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
                entity.HasData(new { Id = 3, OperatorName = "Telenor - NO", Country = "nb", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
                entity.HasData(new { Id = 4, OperatorName = "Telenor - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });

            });

            modelBuilder.ApplyConfiguration(new CustomerOperatorAccountConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new DatapackageConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new OperatorConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionAddOnProductConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionOrderConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionProductConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerOperatorSettingsConfiguration(isSqlLite));

        }
    }
}
