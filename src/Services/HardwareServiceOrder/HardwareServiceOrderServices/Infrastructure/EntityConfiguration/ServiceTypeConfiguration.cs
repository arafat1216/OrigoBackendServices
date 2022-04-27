using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardwareServiceOrderServices.Models;
using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        private readonly bool _isSqlLite;

        public ServiceTypeConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            /*
             * Properties
             */

            builder.Property(s => s.CreatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(s => s.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()")
                   .ValueGeneratedOnAddOrUpdate()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        }
    }
}
