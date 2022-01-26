using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            builder.ToTable("Operator");
            
            //Properties
            builder.Property(x => x.OperatorName).HasMaxLength(50);
            builder.Property(x => x.Country).HasMaxLength(2).HasColumnType("char");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");


            //Relationships
            builder.OwnsMany(h => h.SubscriptionTypes);

        }
    }
}
