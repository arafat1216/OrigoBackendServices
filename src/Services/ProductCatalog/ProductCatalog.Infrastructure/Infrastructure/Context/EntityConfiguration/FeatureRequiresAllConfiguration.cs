using FeatureCatalog.Infrastructure.Models.Database.Joins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureRequiresAllConfiguration : IEntityTypeConfiguration<FeatureRequiresAll>
    {
        public void Configure(EntityTypeBuilder<FeatureRequiresAll> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.FeatureId, e.RequiresFeatureId });

            /*
             * Properties
             */

            builder.Property(e => e.FeatureId)
                   .HasColumnOrder(0)
                   .HasComment("The feature that has requirements");

            builder.Property(e => e.RequiresFeatureId)
                   .HasColumnOrder(1)
                   .HasComment("The 'FeatureID' requires all these features before it can be added or used. This is a one-way requirement");

            /*
             * Relationships / Navigation
             */

            builder.HasOne(fe => fe.Feature)
                   .WithMany(f => f.RequiresAll)
                   .HasForeignKey(fe => fe.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.RequiresFeature)
                   .WithMany(f => f.HasRequiresAllDependenciesFrom)
                   .HasForeignKey(fe => fe.RequiresFeatureId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
