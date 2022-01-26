using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure.EntityConfiguration;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagmentContext : DbContext
    {
        public SubscriptionManagmentContext(DbContextOptions<SubscriptionManagmentContext> options) : base(options)
        {

        }
        public DbSet<Operator> OperatorTypes { get; set; } = null!;

        public DbSet<OperatorAccount> OperatorAccounts { get; set; } = null!;

        public DbSet<SubscriptionProduct> SubscriptionTypes { get; set; } = null!;

        public DbSet<SubscriptionOrder> Subscriptions { get; set; } = null!;

        public DbSet<Datapackage> Datapackages { get; set; } = null!;
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Operator type tables
            modelBuilder.ApplyConfiguration(new OperatorConfiguration());

            // OperatorAccount tables
            modelBuilder.ApplyConfiguration(new OperatorAccountConfiguration());

            // Subscription type tables
            modelBuilder.ApplyConfiguration(new SubscriptionProductConfiguration());

            // Subscription tables
            modelBuilder.ApplyConfiguration(new SubscriptionOrderConfiguration());

            // Datapackage tables
            modelBuilder.ApplyConfiguration(new DatapackageConfiguration());
            // Datapackage tables
            modelBuilder.ApplyConfiguration(new SubscriptionAddOnProductConfiguration());

            //modelBuilder.SeedSubscriptionManagementDb();
        }
    }
}
