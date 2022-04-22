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

        public DbSet<CustomerSettings> CustomerSettings => Set<CustomerSettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSQLite));
        }
    }
}
