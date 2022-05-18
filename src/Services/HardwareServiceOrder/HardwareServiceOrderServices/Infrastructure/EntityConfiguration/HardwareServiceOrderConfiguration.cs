using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class HardwareServiceOrderConfiguration : IEntityTypeConfiguration<HardwareServiceOrder>
    {
        private readonly bool _isSqlLite;

        public HardwareServiceOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<HardwareServiceOrder> builder)
        {
            var comparer = new ValueComparer<string>(
               (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
               v => v.ToUpperInvariant().GetHashCode(),
               v => v
           );

            builder.HasAlternateKey(e => e.ExternalId);
            builder.HasIndex(e => e.AssetLifecycleId);

            /*
             * Properties
             */
            if (_isSqlLite)
            {
                builder.Property(e => e.CreatedDate)
                    .HasConversion(new DateTimeOffsetToBinaryConverter())
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Property(e => e.LastUpdatedDate)
                   .HasConversion(new DateTimeOffsetToBinaryConverter())
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }
            else
            {
                builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Property(e => e.LastUpdatedDate)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }

            builder.OwnsOne(m => m.DeliveryAddress)
                .Property(e => e.Country)
                .HasComment("The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard.")
                .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false)
                   .Metadata.SetValueComparer(comparer);
        }
    }
}
