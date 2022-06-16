using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    internal class CustomerSettingConfiguration : IEntityTypeConfiguration<CustomerSettings>
    {
        private readonly bool _isSqLite;

        public CustomerSettingConfiguration(bool isSqLite)
        {
            _isSqLite = isSqLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            builder.Property(s => s.LastUpdatedDate)
                .HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.OwnsOne(s=>s.DisposeSetting).OwnsMany(x=>x.ReturnLocations).ToTable(nameof(ReturnLocation));
        }
    }
}
