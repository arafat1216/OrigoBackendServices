using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class CustomerSettingsConfiguration : EntityBaseConfiguration<CustomerSettings>
    {

        public CustomerSettingsConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);

            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLowerInvariant().GetHashCode(),
                v => v
            );


            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.ToTable("CustomerSettings");


            /*
             * Properties
             */

            builder.Property(e => e.LoanDeviceEmail)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);

            builder.Property(e => e.LoanDevicePhoneNumber)
                   .HasComment("A phone-number using E.164 format.")
                   .HasMaxLength(15)
                   .IsUnicode(false);
        }
    }
}
