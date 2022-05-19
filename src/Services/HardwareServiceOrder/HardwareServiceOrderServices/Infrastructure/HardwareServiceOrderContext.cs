using HardwareServiceOrderServices.Infrastructure.EntityConfiguration;
using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderContext : DbContext
    {
        private readonly bool _isSQLite;

        public HardwareServiceOrderContext(DbContextOptions<HardwareServiceOrderContext> options) : base(options)
        {
            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite")) continue;
                _isSQLite = true;
                break;
            }
        }

        public bool IsSQLite => _isSQLite;

        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<HardwareServiceOrder> HardwareServiceOrders { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceStatus> ServiceStatuses { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new HardwareServiceOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new EntityConfiguration.ServiceProviderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceStatusConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceTypeConfiguration(_isSQLite));
        }
    }
}
