using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;
using ProductCatalog.Service.Models.Boilerplate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLower().GetHashCode(),
                v => v
            );

            builder.ToTable(t => t.IsTemporal());

            builder.HasIndex(e => e.PartnerId);

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.ProductId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false)
                       .Metadata.SetValueComparer(comparer);
            });

            builder.HasMany(p => p.Features)
                   .WithMany(f => f.Products)
                   .UsingEntity<ProductFeature>();

            /*
            builder.HasMany(e => e.Excludes).WithMany(e => e.HasExcludesDependencyFrom);
            builder.HasMany(e => e.RequiresAll).WithMany(e => e.HasRequiresAllDependencyFrom);
            builder.HasMany(e => e.RequiresOne).WithMany(e => e.HasRequiresOneDependencyFrom);
            */
        }
    }
}
