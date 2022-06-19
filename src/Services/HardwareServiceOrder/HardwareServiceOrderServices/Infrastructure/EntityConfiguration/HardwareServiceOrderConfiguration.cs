using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

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


            builder.OwnsOne(navigationExpression: o => o.AssetInfo, builder =>
            {
                // Convert the HashSet to a flat item so we don't need a separate table for the values
                builder.Property(e => e.Imei)
                       .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)default),
                            v => JsonSerializer.Deserialize<HashSet<string>?>(v, (JsonSerializerOptions?)default),
                            valueComparer: new ValueComparer<HashSet<string>?>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToHashSet()
                            )
                        );

                // Convert the List to a flat item so we don't need a separate table for the values
                builder.Property(propertyExpression: e => e.Accessories)
                       .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)default),
                            v => JsonSerializer.Deserialize<List<string>?>(v, (JsonSerializerOptions?)default),
                            new ValueComparer<List<string>?>(
                                (c1, c2) => c1!.SequenceEqual(c2),
                                c => c!.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c!.ToList()
                            )
                        );
            });

            // If all nullable properties contain a null value in database then the object instance won't be created in the query. Therefore we set
            // it as required as this enforces the creation, even if all it's properties are null (due to the navigation property being non-nullable)
            builder.Navigation(p => p.AssetInfo)
                   .IsRequired();


            builder.OwnsOne(o => o.ReturnedAssetInfo, builder =>
            {
                // Convert the HashSet to a flat item so we don't need a separate table for the values
                builder.Property(e => e.Imei)
                       .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)default),
                            v => JsonSerializer.Deserialize<HashSet<string>?>(v, (JsonSerializerOptions?)default),
                            valueComparer: new ValueComparer<HashSet<string>?>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToHashSet()
                            )
                        );

                // Convert the List to a flat item so we don't need a separate table for the values
                builder.Property(propertyExpression: e => e.Accessories)
                       .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)default),
                            v => JsonSerializer.Deserialize<List<string>?>(v, (JsonSerializerOptions?)default),
                            new ValueComparer<List<string>?>(
                                (c1, c2) => c1!.SequenceEqual(c2),
                                c => c!.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c!.ToList()
                            )
                        );
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
