using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasMany(u => u.ManagesDepartments).WithMany(d => d.Managers).UsingEntity(join => join.ToTable("DepartmentManager"));
            builder.HasMany(u => u.Departments).WithMany(d => d.Users).UsingEntity(join => join.ToTable("DepartmentUser"));
        }
    }
}
