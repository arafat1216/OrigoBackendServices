using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionAddOnProductConfiguration : IEntityTypeConfiguration<SubscriptionAddOnProduct>
    {
        public void Configure(EntityTypeBuilder<SubscriptionAddOnProduct> builder)
        {
            builder.ToTable("SubscriptionAddOnProduct");

            //Properties
            builder.Property(s=>s.AddOnProductName).HasMaxLength(50);
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");

        }
    }
}
