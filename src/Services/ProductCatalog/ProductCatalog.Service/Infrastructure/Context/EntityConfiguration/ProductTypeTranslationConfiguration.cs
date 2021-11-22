using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductTypeTranslationConfiguration : IEntityTypeConfiguration<ProductTypeTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTypeTranslation> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductTypeId, e.Language });

            builder.Property(e => e.Language)
                   .IsUnicode(false);
        }
    }
}
