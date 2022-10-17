using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class ActivateSimOrderConfiguration : IEntityTypeConfiguration<ActivateSimOrder>
    {
        private readonly bool _isSqlLite;

        public ActivateSimOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ActivateSimOrder> builder)
        {
            builder.ToTable("ActivateSimOrder");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);
            builder.Property(e => e.SubscriptionOrderId)
                   .HasColumnOrder(1);

            builder.Property(e => e.MobileNumber)
               .HasComment("A phone-number using E.164 format.")
               .HasMaxLength(15)
               .IsUnicode(false);
            builder.Property(s => s.SimCardNumber).HasMaxLength(22);
        }
    }
}
