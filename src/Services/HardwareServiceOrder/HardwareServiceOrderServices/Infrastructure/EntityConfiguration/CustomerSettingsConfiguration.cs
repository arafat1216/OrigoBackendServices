using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    public class CustomerSettingsConfiguration : IEntityTypeConfiguration<CustomerSettings>
    {
        private readonly bool _isSqlLite;

        public CustomerSettingsConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            builder.ToTable("CustomerSettings");

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
