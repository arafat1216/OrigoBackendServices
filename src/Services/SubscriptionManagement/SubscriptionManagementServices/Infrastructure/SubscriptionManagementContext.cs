using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;
using System.Reflection;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementContext : DbContext
    {
        public SubscriptionManagementContext(DbContextOptions<SubscriptionManagementContext> options) : base(options)
        {

        }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<CustomerOperatorAccount> CustomerOperatorAccounts { get; set; }
        public DbSet<SubscriptionProduct> SubscriptionProducts { get; set; }
        public DbSet<SubscriptionOrder> SubscriptionOrders { get; set; } 
        public DbSet<Datapackage> Datapackages { get; set; }
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
