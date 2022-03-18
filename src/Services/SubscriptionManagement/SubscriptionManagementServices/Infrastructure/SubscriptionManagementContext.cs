using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure.EntityConfiguration;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.SeedData;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementContext : DbContext
    {
        private readonly bool _isSQLite;
        public SubscriptionManagementContext(DbContextOptions<SubscriptionManagementContext> options) : base(options)
        {
            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite")) continue;
                _isSQLite = true;
                break;
            }
        }

        public bool IsSQLite => _isSQLite;

        public DbSet<Operator> Operators => Set<Operator>();
        public DbSet<CustomerOperatorAccount> CustomerOperatorAccounts => Set<CustomerOperatorAccount>();
        public DbSet<SubscriptionProduct> SubscriptionProducts => Set<SubscriptionProduct>();
        public DbSet<TransferToBusinessSubscriptionOrder> TransferSubscriptionOrders => Set<TransferToBusinessSubscriptionOrder>();
        public DbSet<DataPackage> DataPackages => Set<DataPackage>();
        public DbSet<CustomerSettings> CustomerSettings => Set<CustomerSettings>();
        public DbSet<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrders => Set<TransferToPrivateSubscriptionOrder>();
        public DbSet<ChangeSubscriptionOrder> ChangeSubscriptionOrder => Set<ChangeSubscriptionOrder>();
        public DbSet<CancelSubscriptionOrder> CancelSubscriptionOrders => Set<CancelSubscriptionOrder>();
        public DbSet<ActivateSimOrder> ActivateSimOrders => Set<ActivateSimOrder>();
        public DbSet<OrderSimSubscriptionOrder> OrderSimSubscriptionOrders => Set<OrderSimSubscriptionOrder>();
        public DbSet<NewSubscriptionOrder> NewSubscriptionOrders => Set<NewSubscriptionOrder>();
        public DbSet<CustomerStandardPrivateSubscriptionProduct> CustomerStandardPrivateSubscriptionProducts => Set<CustomerStandardPrivateSubscriptionProduct>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerOperatorAccountConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new DataPackageConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new OperatorConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new SubscriptionAddOnProductConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new SubscriptionProductConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerOperatorSettingsConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new TransferToBusinessSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerSubscriptionProductConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new PrivateSubscriptionConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new BusinessSubscriptionConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new TransferToPrivateSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ChangeSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CancelSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ActivateSimOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new OrderSimSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new NewSubscriptionOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerStandardPrivateSubscriptionProductConfiguration(_isSQLite));
            modelBuilder.SeedGlobalProducts();
        }
    }
}
