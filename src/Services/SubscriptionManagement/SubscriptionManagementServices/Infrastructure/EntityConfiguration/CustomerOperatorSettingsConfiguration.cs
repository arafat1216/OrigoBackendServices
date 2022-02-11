using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerOperatorSettingsConfiguration : IEntityTypeConfiguration<CustomerOperatorSettings>
    {
            private bool _isSqlLite;
            public CustomerOperatorSettingsConfiguration(bool isSqlLite)
            {
                _isSqlLite = isSqlLite;
            }

        public void Configure(EntityTypeBuilder<CustomerOperatorSettings> builder)
        {
            builder.ToTable("CustomerOperatorSettings");

            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            //Relationships
            builder
                .HasOne(e => e.Operator)
                .WithMany(e => e.CustomerOperatorSettings)
                .HasForeignKey(e => e.OperatorId);

            builder.HasMany(e => e.CustomerOperatorAccounts)
                .WithMany(e => e.CustomerOperatorSettings)
                .UsingEntity(join => join.ToTable("CustomersOperatorAccounts"));

        }
    }
}

