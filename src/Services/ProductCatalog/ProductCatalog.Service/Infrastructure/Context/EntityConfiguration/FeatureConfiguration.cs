using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            /*
            builder.ToTable("Feature");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FeatureTypeId).IsRequired();
            builder.Property(e => e.AccessControlPermissionNode).IsRequired();
            */

            builder.ToTable(t => t.IsTemporal());

            builder.HasAlternateKey(e => e.AccessControlPermissionNode);

            builder.Property(e => e.AccessControlPermissionNode)
                   .HasMaxLength(64);

            builder.OwnsMany(e => e.Translations, builder =>
            {
                builder.ToTable(t => t.IsTemporal());

                builder.HasKey(e => new { e.FeatureId, e.Language });

                builder.Property(e => e.Language)
                       .HasMaxLength(2)       
                       .IsFixedLength()
                       .IsUnicode(false);
            });

            /*
            builder.HasMany(e => e.Excludes).WithMany(e => e.HasExcludesDependencyFrom);
            builder.HasMany(e => e.RequiresAll).WithMany(e => e.HasRequiresAllDependencyFrom);
            builder.HasMany(e => e.RequiresOne).WithMany(e => e.HasRequiresOneDependencyFrom);
            */
        }
    }
}
