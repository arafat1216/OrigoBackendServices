using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class OrderSimSubscriptionOrderConfiguration : IEntityTypeConfiguration<OrderSimSubscriptionOrder>
    {
        private readonly bool _isSqlLite;
        public OrderSimSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<OrderSimSubscriptionOrder> builder)
        {
            builder.ToTable("OrderSimSubscriptionOrder");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        }
    }
}
