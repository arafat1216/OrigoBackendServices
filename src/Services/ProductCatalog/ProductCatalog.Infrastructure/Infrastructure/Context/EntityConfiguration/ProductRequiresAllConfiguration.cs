﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductRequiresAllConfiguration : IEntityTypeConfiguration<ProductRequiresAll>
    {
        public void Configure(EntityTypeBuilder<ProductRequiresAll> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductId, e.RequiresProductId });

            /*
             * Properties
             */

            builder.Property(e => e.ProductId)
                   .HasColumnOrder(0);

            builder.Property(e => e.RequiresProductId)
                   .HasColumnOrder(1);


            /*
             * Relationships / Navigation
             */

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
