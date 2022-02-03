using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerOperatorAccountConfiguration : IEntityTypeConfiguration<CustomerOperatorAccount>
    {
        public void Configure(EntityTypeBuilder<CustomerOperatorAccount> builder)
        {
            builder.ToTable("CustomerOperatorAccount");

            //Properties
            builder.Property(x => x.AccountName).HasMaxLength(50);
            builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            //Relationships
            builder.HasOne(e => e.Operator)
                .WithMany(m => m.CustomerOperatorAccounts)
                .HasForeignKey(m => m.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
