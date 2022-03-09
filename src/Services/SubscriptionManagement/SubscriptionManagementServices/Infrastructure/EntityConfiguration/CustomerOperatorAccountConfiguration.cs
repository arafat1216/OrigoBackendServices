using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerOperatorAccountConfiguration : IEntityTypeConfiguration<CustomerOperatorAccount>
    {
        private readonly bool _isSqlLite;
        public CustomerOperatorAccountConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerOperatorAccount> builder)
        {
            builder.ToTable("CustomerOperatorAccount");

            //Properties
            builder.Property(x => x.AccountName).HasMaxLength(40);
            builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.HasIndex(x => new { x.OrganizationId, x.AccountNumber }).IsUnique();

            //Relationships
            builder.HasOne(e => e.Operator)
                .WithMany(m => m.CustomerOperatorAccounts)
                .HasForeignKey(m => m.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
