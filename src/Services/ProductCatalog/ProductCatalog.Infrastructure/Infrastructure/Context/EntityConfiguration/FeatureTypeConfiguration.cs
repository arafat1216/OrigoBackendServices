using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
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

            /*
             * Owned entities
             */

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.FeatureTypeId, e.Language });

                /*
                 * Properties
                 */

                builder.Property(e => e.FeatureTypeId)
                       .HasColumnOrder(0);

                builder.Property(e => e.Language)
                       .HasColumnOrder(1)
                       .HasComment("The language for this translation. This should be stored in lowercase 'ISO 639-1' format")
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false)
                       .Metadata.SetValueComparer(comparer);

                builder.Property(e => e.Name)
                       .HasColumnOrder(2)
                       .HasComment("A short, descriptive name");

                builder.Property(e => e.Description)
                       .HasColumnOrder(3)
                       .HasComment("An optional description");
            });
        }
    }
}
