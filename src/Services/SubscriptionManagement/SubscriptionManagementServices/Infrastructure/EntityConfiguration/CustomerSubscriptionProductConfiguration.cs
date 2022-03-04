using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerSubscriptionProductConfiguration : IEntityTypeConfiguration<CustomerSubscriptionProduct>
    {
        private readonly bool _isSqlLite;

        public CustomerSubscriptionProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSubscriptionProduct> builder)
        {
            builder.ToTable("CustomerSubscriptionProduct");

            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.HasMany(e => e.DataPackages)
                .WithMany(e => e.CustomerSubscriptionProducts)
                .UsingEntity(join => join.ToTable("CustomersDatapackage"));

            builder.HasOne(e => e.Operator).WithMany(o => o.CustomerSubscriptionProducts)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
