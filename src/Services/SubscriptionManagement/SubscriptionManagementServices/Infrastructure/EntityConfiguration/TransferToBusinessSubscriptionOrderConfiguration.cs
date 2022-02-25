using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class TransferToBusinessSubscriptionOrderConfiguration : IEntityTypeConfiguration<TransferToBusinessSubscriptionOrder>
    {
        private bool _isSqlLite;
        public TransferToBusinessSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<TransferToBusinessSubscriptionOrder> builder)
        {
            builder.ToTable("TransferToBusinessSubscriptionOrder");

            //Properties

            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.SimCardNumber).HasMaxLength(20);

            //Relationships
            builder.HasOne(e => e.OperatorAccount)
                .WithMany(e => e.TransferToBusinessSubscriptionOrders)
                .HasForeignKey(m => m.OperatorAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DataPackage)
                .WithMany(m => m.TransferToBusinessSubscriptionOrders)
                .HasForeignKey(m => m.DataPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CustomerSubscriptionProduct)
                .WithMany(m => m.TransferToBusinessSubscriptionOrders)
                .HasForeignKey(m => m.SubscriptionProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.SubscriptionAddOnProducts)
                .WithMany(m => m.TransferToBusinessSubscriptionOrders)
                .UsingEntity(join => join.ToTable("TransferToBusinessSubscriptionOrderAddOnProducts"));

            builder.HasOne(e => e.PrivateSubscription);

            builder.HasOne(e => e.BusinessSubscription);
        }
    }
}
