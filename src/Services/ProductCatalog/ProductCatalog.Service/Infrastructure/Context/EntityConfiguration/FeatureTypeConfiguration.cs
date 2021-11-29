using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Models.Database;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureTypeConfiguration : IEntityTypeConfiguration<FeatureType>
    {
        public void Configure(EntityTypeBuilder<FeatureType> builder)
        {
            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLower().GetHashCode(),
                v => v
            );

            builder.ToTable(t => t.IsTemporal());

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.FeatureTypeId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false)
                       .Metadata.SetValueComparer(comparer);
            });
        }
    }
}
