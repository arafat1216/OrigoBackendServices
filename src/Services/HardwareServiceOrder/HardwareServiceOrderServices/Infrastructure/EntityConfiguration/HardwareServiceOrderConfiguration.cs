﻿using HardwareServiceOrderServices.Models;
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

            /*
             * Keys, constraints & indexing
             */

            builder.HasAlternateKey(e => e.ExternalId);

            builder.HasIndex(e => e.AssetLifecycleId);
            builder.HasIndex(e => e.CustomerId);

            /*
             * Properties
             */

            if (_isSqlLite)
            {
                builder.Property(e => e.DateCreated)
                       .HasConversion(new DateTimeOffsetToBinaryConverter())
                       .HasDefaultValueSql("CURRENT_TIMESTAMP")
                       .ValueGeneratedOnAdd().Metadata
                       .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Property(e => e.DateUpdated)
                       .HasConversion(new DateTimeOffsetToBinaryConverter())
                       .HasDefaultValueSql("CURRENT_TIMESTAMP")
                       .ValueGeneratedOnAddOrUpdate().Metadata
                       .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }
            else
            {
                builder.Property(e => e.DateCreated)
                       .HasDefaultValueSql("SYSUTCDATETIME()")
                       .ValueGeneratedOnAdd().Metadata
                       .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Property(e => e.DateUpdated)
                       .HasDefaultValueSql("SYSUTCDATETIME()")
                       .ValueGeneratedOnAddOrUpdate().Metadata
                       .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }


            /*
             * Owned Entities
             */

            builder.OwnsOne(o => o.DeliveryAddress, builder =>
            {
                builder.Property(e => e.Country)
                       .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false).Metadata
                       .SetValueComparer(comparer);
            });


            builder.OwnsOne(o => o.Owner, builder =>
            {
                // Configure here as needed...
            });


            builder.OwnsMany(e => e.ServiceEvents, builder =>
            {

                /*
                 * Properties
                 */
                if (_isSqlLite)
                {
                    //Needed for unit testing using sqlite https://stackoverflow.com/questions/69819523/ef-core-owned-entity-shadow-pk-causes-null-constraint-violation-with-sqlite
                    builder.HasKey("Id"); 
                }

                builder.Property(e => e.Timestamp)
                       .HasComment("When this event was recorded in the external service-provider's system");
            });
        }
    }
}
