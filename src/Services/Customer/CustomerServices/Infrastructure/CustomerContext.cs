using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
        }
    }
}