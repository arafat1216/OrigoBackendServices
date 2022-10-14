using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class TransferToBusinessSubscriptionOrderConfiguration : IEntityTypeConfiguration<TransferToBusinessSubscriptionOrder>
    {
        private readonly bool _isSqlLite;
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
            builder.Property(s => s.SimCardNumber).HasMaxLength(22);

            builder.HasMany(m => m.SubscriptionAddOnProducts)
                .WithMany(m => m.TransferToBusinessSubscriptionOrders)
                .UsingEntity(join => join.ToTable("TransferToBusinessSubscriptionOrderAddOnProducts"));

            builder.HasOne(e => e.PrivateSubscription);

            builder.HasOne(e => e.BusinessSubscription);

            builder.Property(e => e.MobileNumber)
               .HasComment("A phone-number using E.164 format.")
               .HasMaxLength(15)
               .IsUnicode(false);
        }
    }
}
