using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="UserPermissions"/> entity.
    /// </summary>
    internal class UserPermissionsConfiguration : IEntityTypeConfiguration<UserPermissions>
    {
        private readonly bool _isSqlLite;

        public UserPermissionsConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<UserPermissions> builder)
        {
            /*
             * Properties
             */

            builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "GETUTCDATE()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "GETUTCDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(userPermissions => userPermissions.AccessList)
                   .HasConversion(
                        convertTo => JsonSerializer.Serialize(convertTo, new JsonSerializerOptions { IgnoreNullValues = true }),
                        convertFrom => JsonSerializer.Deserialize<IList<Guid>>(convertFrom, new JsonSerializerOptions { IgnoreNullValues = true })
                    );
        }
    }
}
