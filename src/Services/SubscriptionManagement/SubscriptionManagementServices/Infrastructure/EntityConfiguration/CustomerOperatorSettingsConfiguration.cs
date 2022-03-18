using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration;

internal class CustomerOperatorSettingsConfiguration : IEntityTypeConfiguration<CustomerOperatorSettings>
{
    private readonly bool _isSqlLite;

    public CustomerOperatorSettingsConfiguration(bool isSqlLite)
    {
        _isSqlLite = isSqlLite;
    }

    public void Configure(EntityTypeBuilder<CustomerOperatorSettings> builder)
    {
        builder.ToTable("CustomerOperatorSettings");

        builder.Property(s => s.LastUpdatedDate)
            .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

        //Relationships
        builder.HasOne(s => s.Operator).WithMany(o => o.CustomerOperatorSettings);

        builder.HasMany(s => s.CustomerOperatorAccounts).WithOne(a => a.CustomerOperatorSetting);

        builder.HasMany(s => s.AvailableSubscriptionProducts).WithOne(a => a.CustomerOperatorSettings).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.StandardPrivateSubscriptionProduct)
        .WithOne(b => b.CustomerOperatorSettings)
        .HasForeignKey<CustomerStandardPrivateSubscriptionProduct>(b => b.CustomerOperatorSettingId);
    }
}