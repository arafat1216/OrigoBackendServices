﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class TransferToBusinessSubscriptionOrderConfiguration : IEntityTypeConfiguration<TransferToBusinessSubscriptionOrder>
    {
        private readonly bool _isSqlLite;
        public TransferToBusinessSubscriptionOrderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<TransferToBusinessSubscriptionOrder> builder)
        {
            builder.ToTable("TransferToBusinessSubscriptionOrder");

            //Properties

            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.SimCardNumber).HasMaxLength(22);

            builder.HasMany(m => m.SubscriptionAddOnProducts)
                .WithMany(m => m.TransferToBusinessSubscriptionOrders)
                .UsingEntity(join => join.ToTable("TransferToBusinessSubscriptionOrderAddOnProducts"));

            builder.HasOne(e => e.PrivateSubscription);

            builder.HasOne(e => e.BusinessSubscription);

            builder.Property(e => e.OperatorAccountPhoneNumber)
               .HasComment("A phone-number using E.164 format.")
               .HasMaxLength(15)
               .IsUnicode(false);

            builder.Property(e => e.SimCardCountry)
                   .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                   .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false);
        }
    }
}
