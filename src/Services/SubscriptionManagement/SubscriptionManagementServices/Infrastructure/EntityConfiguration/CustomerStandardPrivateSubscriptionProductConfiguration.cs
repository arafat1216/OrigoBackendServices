﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;
namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    public class CustomerStandardPrivateSubscriptionProductConfiguration : IEntityTypeConfiguration<CustomerStandardPrivateSubscriptionProduct>
    {
        private readonly bool _isSqlLite;

        public CustomerStandardPrivateSubscriptionProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerStandardPrivateSubscriptionProduct> builder)
        {
            builder.ToTable("CustomerStandardPrivateSubscriptionProduct");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(e => e.Id)
                   .HasColumnOrder(0);
        }
    }
}
