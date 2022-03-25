using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration;

internal class AssetLifecycleConfiguration : IEntityTypeConfiguration<AssetLifecycle>
{
    private readonly bool _isSqLite;

    public AssetLifecycleConfiguration(bool isSqLite)
    {
        _isSqLite = isSqLite;
    }

    public void Configure(EntityTypeBuilder<AssetLifecycle> builder)
    {
        builder.Property(s => s.LastUpdatedDate)
            .HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
    }
}