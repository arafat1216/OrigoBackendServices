using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using CustomerServices.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerContext : DbContext
    {
        private readonly IMediator _mediator;

        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ProductModule> ProductModules { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<ProductModuleGroup> ProductModuleGroups { get; set; }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Customer>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductModule>().ToTable("ProductModule");
        }

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB. This makes
            // a single transaction including side effects from the domain event
            // handlers that are using the same DbContext with Scope lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB. This makes
            // multiple transactions. You will need to handle eventual consistency and
            // compensatory actions in case of failures.
            //await _mediator.DispatchDomainEventsAsync(this);

            // After this line runs, all the changes (from the Command Handler and Domain
            // event handlers) performed through the DbContext will be committed
            return await SaveChangesAsync();
        }
    }
}