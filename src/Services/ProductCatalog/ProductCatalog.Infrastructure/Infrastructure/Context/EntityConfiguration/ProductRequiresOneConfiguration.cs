using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductRequiresOneConfiguration : IEntityTypeConfiguration<ProductRequiresOne>
    {
        public void Configure(EntityTypeBuilder<ProductRequiresOne> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductId, e.RequiresProductId });

            builder.HasOne(pe => pe.Product)
                   .WithMany(p => p.RequiresOne)
                   .HasForeignKey(pe => pe.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pe => pe.RequiresProduct)
                   .WithMany(p => p.HasRequiresOneDependenciesFrom)
                   .HasForeignKey(pe => pe.RequiresProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
