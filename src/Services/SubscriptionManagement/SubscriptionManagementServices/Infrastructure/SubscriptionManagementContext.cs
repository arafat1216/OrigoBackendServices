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

        public DbSet<Operator> Operators => Set<Operator>();
        public DbSet<CustomerOperatorAccount> CustomerOperatorAccounts => Set<CustomerOperatorAccount>();
        public DbSet<SubscriptionProduct> SubscriptionProducts => Set<SubscriptionProduct>();
        public DbSet<SubscriptionOrder> SubscriptionOrders => Set<SubscriptionOrder>();
        public DbSet<TransferToBusinessSubscriptionOrder> TransferSubscriptionOrders => Set<TransferToBusinessSubscriptionOrder>();
        public DbSet<DataPackage> DataPackages => Set<DataPackage>();
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts => Set<SubscriptionAddOnProduct>();
        public DbSet<CustomerSettings> CustomerSettings => Set<CustomerSettings>();
        public DbSet<CustomerOperatorSettings> CustomerOperatorSettings => Set<CustomerOperatorSettings>();
        public DbSet<CustomerSubscriptionProduct> CustomerSubscriptionProducts => Set<CustomerSubscriptionProduct>();
        public DbSet<PrivateSubscription> PrivateSubscriptions => Set<PrivateSubscription>();
        public DbSet<BusinessSubscription> BusinessSubscriptions => Set<BusinessSubscription>();
        public DbSet<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrders => Set<TransferToPrivateSubscriptionOrder>();

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
            modelBuilder.ApplyConfiguration(new TransferToBusinessSubscriptionOrderConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerSubscriptionProductConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new PrivateSubscriptionConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new BusinessSubscriptionConfiguration(_isSqlLite));
            modelBuilder.ApplyConfiguration(new TransferToPrivateSubscriptionOrderConfiguration(_isSqlLite));
        }
    }
}
