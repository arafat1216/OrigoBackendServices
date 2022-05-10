using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    internal class LifeCycleSettingConfiguration : IEntityTypeConfiguration<LifeCycleSetting>
    {
        private readonly bool _isSqLite;

        public LifeCycleSettingConfiguration(bool isSqLite)
        {
            _isSqLite = isSqLite;
        }

        public void Configure(EntityTypeBuilder<LifeCycleSetting> builder)
        {
            builder.Property(a => a.MinBuyoutPrice).HasColumnType("decimal(18,2)");
            builder.Property(s => s.LastUpdatedDate)
                .HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        }
    }
}
