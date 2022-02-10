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
        public DbSet<TransferSubscriptionOrder> TransferSubscriptionOrders { get; set; }
        public DbSet<Datapackage> Datapackages { get; set; }
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<CustomerOperatorSettings> CustomerOperatorSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerOperatorAccountConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new DatapackageConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new OperatorConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionAddOnProductConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionOrderConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new SubscriptionProductConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new CustomerOperatorSettingsConfiguration(isSqlLite));
            modelBuilder.ApplyConfiguration(new TransferSubscriptionOrderConfiguration(isSqlLite));
        }
    }
}
