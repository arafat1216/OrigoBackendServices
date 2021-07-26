using Common.Extensions;
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

        public DbSet<AssetCategoryLifecycleType> AssetCategoryLifecycleTypes { get; set; }

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
            modelBuilder.Entity<AssetCategoryLifecycleType>().ToTable("AssetCategoryLifecycleType");
            modelBuilder.Entity<Customer>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>().Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductModule>().ToTable("ProductModule");
        }

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var numberOfRecordsSaved = await SaveChangesAsync();
            await _mediator.DispatchDomainEventsAsync(this);
            return numberOfRecordsSaved;
        }
    }
}