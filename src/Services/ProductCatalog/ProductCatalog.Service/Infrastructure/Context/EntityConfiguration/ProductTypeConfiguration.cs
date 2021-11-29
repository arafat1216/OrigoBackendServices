using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
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

                builder.HasKey(e => new { e.ProductTypeId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false)
                       .Metadata.SetValueComparer(comparer);
            });
        }
    }
}
