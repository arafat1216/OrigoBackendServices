using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;


namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class ChangeSubscriptionOrderConfiguration : IEntityTypeConfiguration<ChangeSubscriptionOrder>
    {
        private readonly bool _isSqlLite;
        public ChangeSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ChangeSubscriptionOrder> builder)
        {
            builder.ToTable("ChangeSubscriptionOrder");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);
            builder.Property(e => e.SubscriptionOrderId)
                   .HasColumnOrder(1);
        }
    }
}
