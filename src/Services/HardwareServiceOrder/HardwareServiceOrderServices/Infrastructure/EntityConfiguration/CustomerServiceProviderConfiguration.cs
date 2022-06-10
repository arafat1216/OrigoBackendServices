using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    public class CustomerServiceProviderConfiguration : IEntityTypeConfiguration<CustomerServiceProvider>
    {
        private readonly bool _isSqlLite;

        public CustomerServiceProviderConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerServiceProvider> builder)
        {
            builder.ToTable("CustomerServiceProvider", table => table.IsTemporal());

            builder.HasKey(x => new { x.CustomerId, x.Id, x.AssetCategoryId, x.ServiceProviderId });

            /*
             * Properties
             */

            builder.Property(e => e.DateCreated)
                  .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.DateUpdated)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
