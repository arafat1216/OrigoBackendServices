using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureTypeConfiguration : IEntityTypeConfiguration<FeatureType>
    {
        public void Configure(EntityTypeBuilder<FeatureType> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.FeatureTypeId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)
                       .IsFixedLength()
                       .IsUnicode(false);
            });
        }
    }
}
