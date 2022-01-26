using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class OperatorAccountConfiguration : IEntityTypeConfiguration<OperatorAccount>
    {
        public void Configure(EntityTypeBuilder<OperatorAccount> builder)
        {
            builder.ToTable("OperatorAccount");

            //Properties
            builder.Property(x => x.AccountName).HasMaxLength(50);
            builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();


            //Relationships
            builder.HasOne(e => e.OperatorTypeId);

            builder.HasMany(e=>e.Subscriptions)
                    .WithOne(e =>e.OperatorAccount);
        }
    }
}
