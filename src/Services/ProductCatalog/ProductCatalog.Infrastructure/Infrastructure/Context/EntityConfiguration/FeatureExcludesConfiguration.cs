using FeatureCatalog.Infrastructure.Models.Database.Joins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureExcludesConfiguration : IEntityTypeConfiguration<FeatureExcludes>
    {
        public void Configure(EntityTypeBuilder<FeatureExcludes> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.FeatureId, e.ExcludesFeatureId });

            builder.HasOne(fe => fe.Feature)
                   .WithMany(f => f.Excludes)
                   .HasForeignKey(fe => fe.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.RequiresFeature)
                   .WithMany(f => f.HasExcludesDependenciesFrom)
                   .HasForeignKey(pe => pe.ExcludesFeatureId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
