using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class BusinessSubscriptionConfiguration : IEntityTypeConfiguration<BusinessSubscription>
    {
        private readonly bool _isSqlLite;
        public BusinessSubscriptionConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<BusinessSubscription> builder)
        {
            builder.ToTable("BusinessSubscription");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(e => e.Country)
                   .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                   .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false);
        }
    }
}
