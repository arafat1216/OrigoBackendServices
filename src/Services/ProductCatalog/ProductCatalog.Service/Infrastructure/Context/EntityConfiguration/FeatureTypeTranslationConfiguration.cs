using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureTypeTranslationConfiguration : IEntityTypeConfiguration<FeatureTypeTranslation>
    {
        public void Configure(EntityTypeBuilder<FeatureTypeTranslation> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.FeatureTypeId, e.Language });

            builder.Property(e => e.Language)
                   .IsUnicode(false);
        }
    }
}
