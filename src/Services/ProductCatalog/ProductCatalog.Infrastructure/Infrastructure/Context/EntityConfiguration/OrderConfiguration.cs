using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasAlternateKey(e => e.ExternalId);
            builder.HasIndex(e => e.OrganizationId);

            /*
             * Properties
             */

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);

            builder.Property(e => e.ExternalId)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("newid()")
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            builder.Property(e => e.ProductId)
                   .HasColumnOrder(2);

            builder.Property(e => e.OrganizationId)
                   .HasColumnOrder(3);
        }
    }
}
