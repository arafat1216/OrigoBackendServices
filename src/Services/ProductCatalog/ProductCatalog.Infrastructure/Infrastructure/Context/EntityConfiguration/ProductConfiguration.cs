using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
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

            /*
             * Properties
             */

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);

            builder.Property(e => e.PartnerId)
                   .HasColumnOrder(1)
                   .HasComment("The partner that owns and sells this product");

            builder.Property(e => e.ProductTypeId)
                   .HasColumnOrder(2);

            /*
             * Owned entities
             */

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.ProductId, e.Language });

                /*
                 * Properties
                 */

                builder.Property(e => e.ProductId)
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

            /*
             * Relationships / Navigation
             */

            builder.HasMany(p => p.Features)
                   .WithMany(f => f.Products)
                   .UsingEntity<ProductFeature>();

            builder.HasMany(e => e.Excludes)
                   .WithOne(e => e.Product);

            builder.HasMany(e => e.RequiresAll)
                   .WithOne(e => e.Product);

            builder.HasMany(e => e.RequiresOne)
                   .WithOne(e => e.Product);

            // Enable eager loading on requirements
            builder.Navigation(e => e.Excludes).AutoInclude();
            builder.Navigation(e => e.RequiresAll).AutoInclude();
            builder.Navigation(e => e.RequiresOne).AutoInclude();
        }




    }
}
