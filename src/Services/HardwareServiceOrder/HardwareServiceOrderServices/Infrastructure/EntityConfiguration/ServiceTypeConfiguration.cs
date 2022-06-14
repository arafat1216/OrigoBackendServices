using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        private readonly bool _isSqlLite;

        public ServiceTypeConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            /*
             * Properties
             */

            builder.Property(e => e.DateCreated)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.DateUpdated)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
