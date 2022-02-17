using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure.EntityConfiguration;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementContext : DbContext
    {
        private readonly bool _isSqlLite;
        public SubscriptionManagementContext(DbContextOptions<SubscriptionManagementContext> options) : base(options)
        {
            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite")) continue;
                _isSqlLite = true;
                break;
            }
        }

        public DbSet<Operator> Operators { get; set; }
        public DbSet<CustomerOperatorAccount> CustomerOperatorAccounts { get; set; }
        public DbSet<SubscriptionProduct> SubscriptionProducts { get; set; }
        public DbSet<SubscriptionOrder> SubscriptionOrders { get; set; }
        public DbSet<TransferSubscriptionOrder> TransferSubscriptionOrders { get; set; }
        public DbSet<DataPackage> DataPackages { get; set; }
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<CustomerOperatorSettings> CustomerOperatorSettings { get; set; }
        public DbSet<CustomerSubscriptionProduct> CustomerSubscriptionProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerOperatorAccountConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new DataPackageConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new OperatorConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionAddOnProductConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionOrderConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionProductConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerOperatorSettingsConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new TransferSubscriptionOrderConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerSubscriptionProductConfiguration(_isSqlLite));

        }
    }
}
