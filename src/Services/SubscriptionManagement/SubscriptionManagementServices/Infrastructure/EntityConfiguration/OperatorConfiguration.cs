using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

// ReSharper disable StringLiteralTypo

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        private readonly bool _isSqlLite;
        public OperatorConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }
        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            builder.ToTable("Operator");

            //Properties
            builder.Property(x => x.OperatorName).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Country).HasMaxLength(2).IsRequired().HasColumnType("char");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            Guid createdById = Guid.Parse("00000000-0000-0000-0000-000000000000");
            builder.HasData(new { Id = 1, OperatorName = "Telia - NO", Country = "nb", CreatedBy = createdById, CreatedDate = DateTime.Parse("2022-02-09 13:10:02.0474381"), DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
            builder.HasData(new { Id = 2, OperatorName = "Telia - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Parse("2022-02-09 13:10:02.0474381"), DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
            builder.HasData(new { Id = 3, OperatorName = "Telenor - NO", Country = "nb", CreatedBy = createdById, CreatedDate = DateTime.Parse("2022-02-09 13:10:02.0474381"), DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
            builder.HasData(new { Id = 4, OperatorName = "Telenor - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Parse("2022-02-09 13:10:02.0474381"), DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
        }
    }
}