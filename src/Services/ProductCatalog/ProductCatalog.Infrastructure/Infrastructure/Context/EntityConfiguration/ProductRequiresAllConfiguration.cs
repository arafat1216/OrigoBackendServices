using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductRequiresAllConfiguration : IEntityTypeConfiguration<ProductRequiresAll>
    {
        public void Configure(EntityTypeBuilder<ProductRequiresAll> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductId, e.RequiresProductId });

            builder.HasOne(pe => pe.Product)
                   .WithMany(p => p.RequiresAll)
                   .HasForeignKey(pe => pe.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pe => pe.RequiresProduct)
                   .WithMany(p => p.HasRequiresAllDependenciesFrom)
                   .HasForeignKey(pe => pe.RequiresProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
