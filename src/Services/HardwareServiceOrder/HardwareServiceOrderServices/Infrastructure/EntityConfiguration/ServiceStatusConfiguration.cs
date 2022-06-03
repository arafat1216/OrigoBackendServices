using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceStatusConfiguration : IEntityTypeConfiguration<ServiceStatus>
    {
        private readonly bool _isSqlLite;

        public ServiceStatusConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ServiceStatus> builder)
        {
            builder.ToTable(table => table.IsTemporal());

            /*
             * Properties
             */

            builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
