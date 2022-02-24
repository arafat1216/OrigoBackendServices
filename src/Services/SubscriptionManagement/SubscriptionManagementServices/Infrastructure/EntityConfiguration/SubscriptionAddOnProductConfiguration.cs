using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionAddOnProductConfiguration : IEntityTypeConfiguration<SubscriptionAddOnProduct>
    {
        private bool _isSqlLite;
        public SubscriptionAddOnProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<SubscriptionAddOnProduct> builder)
        {
            builder.ToTable("SubscriptionAddOnProduct");

            //Properties
            builder.Property(s=>s.AddOnProductName).HasMaxLength(50).IsRequired();
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        }
    }
}
