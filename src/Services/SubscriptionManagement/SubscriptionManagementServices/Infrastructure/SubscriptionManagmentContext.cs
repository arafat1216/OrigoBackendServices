using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;
using System.Reflection;

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
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
