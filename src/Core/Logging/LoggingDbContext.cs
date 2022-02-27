using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Logging
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }

        public DbSet<FunctionalEventLogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FunctionalEventLogEntry>(ConfigureFunctionalEventLogEntry);
        }

        private void ConfigureFunctionalEventLogEntry(EntityTypeBuilder<FunctionalEventLogEntry> builder)
        {
            builder.ToTable("FunctionalEventLog");
        }
    }
}
