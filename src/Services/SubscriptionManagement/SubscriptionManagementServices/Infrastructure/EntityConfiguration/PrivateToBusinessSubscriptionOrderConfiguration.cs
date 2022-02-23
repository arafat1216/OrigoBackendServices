using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class PrivateToBusinessSubscriptionOrderConfiguration : IEntityTypeConfiguration<PrivateToBusinessSubscriptionOrder>
    {
        private bool _isSqlLite;
        public PrivateToBusinessSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<PrivateToBusinessSubscriptionOrder> builder)
        {
            builder.ToTable("PrivateToBusinessSubscriptionOrder");

            //Properties
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        }
    }
}
