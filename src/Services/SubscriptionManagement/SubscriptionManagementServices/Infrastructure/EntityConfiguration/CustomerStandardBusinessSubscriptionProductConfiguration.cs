

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class CustomerStandardBusinessSubscriptionProductConfiguration : IEntityTypeConfiguration<CustomerStandardBusinessSubscriptionProduct>
    {
        private readonly bool _isSqlLite;

        public CustomerStandardBusinessSubscriptionProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerStandardBusinessSubscriptionProduct> builder)
        {
            builder.ToTable("CustomerStandardBusinessSubscriptionProduct");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);

            builder.HasMany(a => a.AddOnProducts).WithMany(a => a.CustomerStandardBusinessSubscriptionProduct);
        }
    }
}
