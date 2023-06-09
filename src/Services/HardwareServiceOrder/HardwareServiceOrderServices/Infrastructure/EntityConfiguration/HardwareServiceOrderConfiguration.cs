﻿using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class HardwareServiceOrderConfiguration : EntityBaseConfiguration<HardwareServiceOrder>
    {

        public HardwareServiceOrderConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<HardwareServiceOrder> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);

            var toUpperCaseComparer = new ValueComparer<string>(
               (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
               v => v.ToUpperInvariant().GetHashCode(),
               v => v
            );

            
            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.HasAlternateKey(e => e.ExternalId);

            builder.HasIndex(e => e.AssetLifecycleId);
            builder.HasIndex(e => e.CustomerId);


            /*
             * Properties
             */

            // Convert the list to a flat JSON vale so we don't need a separate table for the values
            builder.Property(e => e.IncludedServiceOrderAddonIds)
                   .HasConversion(entity => JsonSerializer.Serialize(entity, (JsonSerializerOptions?)default),
                                  json => JsonSerializer.Deserialize<List<int>?>(json, (JsonSerializerOptions?)default)
                    );

            builder.Navigation(e => e.ServiceEvents)
                   .AutoInclude();


            /*
             * Relationships
             */

            builder.HasMany(e => e.ServiceEvents)
                   .WithOne(e => e.HardwareServiceOrder)
                   .HasForeignKey(e => e.HardwareServiceOrderId);


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
                       .SetValueComparer(toUpperCaseComparer);
            });


            builder.OwnsOne(o => o.Owner, builder =>
            {
                /*
                 * DB table configuration (keys, constraints, indexing, etc.)
                 */

                builder.HasIndex(e => e.UserId);

                builder.Property(e => e.PhoneNumber)
                       .HasComment("A phone-number using E.164 format.")
                       .HasMaxLength(15)
                       .IsUnicode(false);
            });


            builder.OwnsOne(navigationExpression: o => o.AssetInfo, builder =>
            {
                // Convert the HashSet to a flat item so we don't need a separate table for the values
                builder.Property(e => e.Imei)
                       .HasComment("A JSON-serialized list that contains all the device's IMEI numbers.")
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
                       .HasComment("A JSON-serialized list detailing what accessories, if any, was included in the shipment.")
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
                       .HasComment("A JSON-serialized list that contains all the device's IMEI numbers. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.")
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
                       .HasComment("A JSON-serialized list detailing what accessories, if any, was included in the shipment. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.")
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


        }
    }
}
