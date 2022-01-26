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
            builder.HasOne(e => e.OperatorAccount);

            builder.HasOne(e => e.DataPackage);

            builder.HasOne(e => e.SubscriptionType);

            builder.OwnsMany(e=> e.SubscriptionAddOnProducts);
        }
    }
}
