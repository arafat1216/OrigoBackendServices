using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.ProductTypeId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false);
            });
        }
    }
}
