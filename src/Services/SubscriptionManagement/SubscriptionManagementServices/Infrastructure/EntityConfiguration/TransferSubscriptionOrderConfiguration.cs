using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class TransferSubscriptionOrderConfiguration : IEntityTypeConfiguration<TransferSubscriptionOrder>
    {
        private bool _isSqlLite;
        public TransferSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<TransferSubscriptionOrder> builder)
        {
            builder.ToTable("TransferSubscriptionOrder");

            //Properties
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.HasOne(e => e.NewOperatorAccount)
                .WithMany(e => e.TransferSubscriptionOrders)
                .HasForeignKey(m => m.NewOperatorAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
