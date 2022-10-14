using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Organization"/> entity.
    /// </summary>
    internal class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        private readonly bool _isSqlLite;

        public OrganizationConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLowerInvariant().GetHashCode(),
                v => v
            );

            builder.ToTable("Organization");

            builder.HasAlternateKey(e => e.OrganizationId);

            /*
             * Properties
             */
            builder.Property(a => a.LastDayForReportingSalaryDeduction).HasColumnType("int").HasDefaultValue(1);

            builder.Property(e => e.OrganizationId)
                   .HasDefaultValueSql("NEWID()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            builder.OwnsOne(o => o.Address, builder =>
            {
                /*
                 * DB table configuration (keys, constraints, indexing, etc.)
                 */
                builder.Property(e => e.Country)
                   .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                   .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false);
            });


            builder.OwnsOne(o => o.ContactPerson, builder =>
            {
                /*
                 * DB table configuration (keys, constraints, indexing, etc.)
                 */
                builder.Property(e => e.Email)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);

                builder.Property(e => e.PhoneNumber)
                   .HasComment("A phone-number using E.164 format.")
                   .HasMaxLength(15)
                   .IsUnicode(false);
            });

            builder.Property(e => e.PayrollContactEmail)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);
        }
    }
}
