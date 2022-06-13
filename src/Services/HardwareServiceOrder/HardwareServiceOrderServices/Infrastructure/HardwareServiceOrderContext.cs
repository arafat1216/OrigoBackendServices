using Common.EntityFramework;
using HardwareServiceOrderServices.Infrastructure.EntityConfiguration;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.SeedData;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderContext : DbContext
    {
        private readonly bool _isSQLite;
        private readonly IApiRequesterService? apiRequesterService;

        public bool IsSQLite => _isSQLite;

        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<HardwareServiceOrder> HardwareServiceOrders { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceStatus> ServiceStatuses { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<CustomerServiceProvider> CustomerServiceProviders { get; set; }


        public HardwareServiceOrderContext(DbContextOptions<HardwareServiceOrderContext> options, IApiRequesterService? apiRequesterService = null) : base(options)
        {
            this.apiRequesterService = apiRequesterService;

            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite"))
                    continue;

                _isSQLite = true;
                break;
            }
        }


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new HardwareServiceOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new EntityConfiguration.ServiceProviderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceStatusConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceTypeConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerServiceProviderConfiguration(_isSQLite));
            modelBuilder.SeedServiceStatus();
            modelBuilder.SeedServiceType();
            modelBuilder.SeedServiceProvider();
        }


        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_isSQLite is false)
                optionsBuilder.AddInterceptors(new SaveContextChangesInterceptor(apiRequesterService?.AuthenticatedUserId));
        }

    }
}
