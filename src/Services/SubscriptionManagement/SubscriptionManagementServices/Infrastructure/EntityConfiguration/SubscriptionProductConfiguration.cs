using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;


namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionProductConfiguration : IEntityTypeConfiguration<SubscriptionProduct>
    {
        private bool _isSqlLite;
        public SubscriptionProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<SubscriptionProduct> builder)
        {
            builder.ToTable("SubscriptionProduct");

            //Properties
            builder.Property(x => x.SubscriptionName).HasMaxLength(50);
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            //Relationships
            builder.HasOne(e => e.Operator)
                .WithMany(m => m.SubscriptionProducts)
                .HasForeignKey(m => m.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}