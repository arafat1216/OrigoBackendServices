using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
    {
        private readonly bool _isSqlLite;

        public ServiceProviderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {

            /*
             * Properties
             */

            builder.Property(s => s.CreatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(s => s.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


        }
    }
}
