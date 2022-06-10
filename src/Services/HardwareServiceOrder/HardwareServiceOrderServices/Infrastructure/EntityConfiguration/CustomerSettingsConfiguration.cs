﻿using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    public class CustomerSettingsConfiguration : IEntityTypeConfiguration<CustomerSettings>
    {
        private readonly bool _isSqlLite;

        public CustomerSettingsConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
            // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToLowerInvariant().GetHashCode(),
                v => v
            );

            builder.ToTable("CustomerSettings", table => table.IsTemporal());

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

            builder.Property(e => e.LoanDeviceEmail)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);

            builder.Property(e => e.LoanDevicePhoneNumber)
                   .HasComment("A phone-number using E.164 format.")
                   .HasMaxLength(15)
                   .IsUnicode(false);
        }
    }
}
