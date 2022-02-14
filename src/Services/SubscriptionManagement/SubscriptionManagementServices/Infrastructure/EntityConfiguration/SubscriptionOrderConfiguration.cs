using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionOrderConfiguration : IEntityTypeConfiguration<SubscriptionOrder>
    {
        private bool _isSqlLite;
        public SubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<SubscriptionOrder> builder)
        {
            builder.ToTable("SubscriptionOrder");

            //Properties
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.SimCardNumber).HasMaxLength(20);

            //Relationships
            builder.HasOne(e => e.OperatorAccount)
                .WithMany(e => e.SubscriptionOrders)
                .HasForeignKey(m => m.OperatorAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DataPackage)
                .WithMany(m => m.SubscriptionOrders)
                .HasForeignKey(m => m.DataPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SubscriptionProduct)
                .WithMany(m => m.SubscriptionOrders)
                .HasForeignKey(m => m.SubscriptionProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
