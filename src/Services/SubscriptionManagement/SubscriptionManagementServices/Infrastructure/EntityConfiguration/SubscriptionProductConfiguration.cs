using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;


namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class SubscriptionProductConfiguration : IEntityTypeConfiguration<SubscriptionProduct>
    {
        public void Configure(EntityTypeBuilder<SubscriptionProduct> builder)
        {
            builder.ToTable("SubscriptionProduct");

            //Properties
            builder.Property(x => x.SubscriptionName).HasMaxLength(50);
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");


            //Relationships
            builder.HasOne(e=>e.OperatorType);

            builder.HasMany(e=>e.DataPackages);
        }
    }
}