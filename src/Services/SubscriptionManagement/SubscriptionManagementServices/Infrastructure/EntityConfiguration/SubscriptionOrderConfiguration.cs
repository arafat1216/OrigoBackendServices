using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionOrderConfiguration : IEntityTypeConfiguration<SubscriptionOrder>
    {
        public void Configure(EntityTypeBuilder<SubscriptionOrder> builder)
        {
            builder.ToTable("SubscriptionOrder");

            //Properties
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");

            //Relationships
            builder.HasOne(e => e.OperatorAccount)
                .WithMany(e => e.SubscriptionOrders)
                .HasForeignKey(m => m.OperatorAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DataPackage)
                .WithMany(m => m.SubscriptionOrders)
                .HasForeignKey(m => m.DatapackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SubscriptionProduct)
                .WithMany(m => m.SubscriptionOrders)
                .HasForeignKey(m => m.SubscriptionProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
