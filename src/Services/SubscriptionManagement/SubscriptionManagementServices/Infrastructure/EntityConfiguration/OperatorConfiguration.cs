using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        private bool _isSqlLite;
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

        }
    }
}
