using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class TransferToPrivateSubscriptionOrderConfiguration : IEntityTypeConfiguration<TransferToPrivateSubscriptionOrder>
    {
        private bool _isSqlLite;
        public TransferToPrivateSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<TransferToPrivateSubscriptionOrder> builder)
        {
            builder.ToTable("TransferToPrivateSubscriptionOrder");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.HasOne(m => m.UserInfo);
        }
    }
}
