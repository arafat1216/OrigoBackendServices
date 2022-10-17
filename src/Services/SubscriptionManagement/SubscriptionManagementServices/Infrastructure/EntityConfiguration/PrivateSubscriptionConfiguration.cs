using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class PrivateSubscriptionConfiguration : IEntityTypeConfiguration<PrivateSubscription>
    {
        private readonly bool _isSqlLite;
        public PrivateSubscriptionConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<PrivateSubscription> builder)
        {
            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLowerInvariant().GetHashCode(),
                v => v
            );

            builder.ToTable("PrivateSubscription");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.HasOne(m => m.RealOwner);
            builder.Property(e => e.Country)
                   .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                   .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false);

            builder.Property(e => e.Email)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);
        }
    }
}
