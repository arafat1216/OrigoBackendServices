using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerSubscriptionProductConfiguration : IEntityTypeConfiguration<CustomerSubscriptionProduct>
    {
        private bool _isSqlLite;

        public CustomerSubscriptionProductConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSubscriptionProduct> builder)
        {
            builder.ToTable("CustomerSubscriptionProduct");

            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

        }
    }
}
