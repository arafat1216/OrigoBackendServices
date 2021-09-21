using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure
{
    public class CustomerConfiguration :  IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}
