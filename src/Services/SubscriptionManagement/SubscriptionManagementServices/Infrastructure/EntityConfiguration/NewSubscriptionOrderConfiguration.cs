using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class NewSubscriptionOrderConfiguration : IEntityTypeConfiguration<NewSubscriptionOrder>
    {
        private readonly bool _isSqlLite;
        public NewSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<NewSubscriptionOrder> builder)
        {
            builder.ToTable("NewSubscriptionOrder");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(s => s.SimCardNumber).HasMaxLength(22);

            builder.HasMany(m => m.SubscriptionAddOnProducts)
                .WithMany(m => m.NewSubscriptionOrders)
                .UsingEntity(join => join.ToTable("NewSubscriptionOrderAddOnProducts"));

            builder.HasOne(e => e.PrivateSubscription);

            builder.HasOne(e => e.BusinessSubscription);

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);
            builder.Property(e => e.SubscriptionOrderId)
                   .HasColumnOrder(1);

        }
    }
}
