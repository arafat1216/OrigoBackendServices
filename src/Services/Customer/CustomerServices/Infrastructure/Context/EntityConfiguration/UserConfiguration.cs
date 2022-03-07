using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="User"/> entity.
    /// </summary>
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            /*
             * Properties
             */

            builder.Property(s => s.LastUpdatedDate)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            /*
             * Relationships / Navigation
             */

            builder.HasMany(u => u.ManagesDepartments)
                   .WithMany(d => d.Managers)
                   .UsingEntity(join => join.ToTable("DepartmentManager"));

            builder.HasMany(u => u.Departments)
                   .WithMany(d => d.Users)
                   .UsingEntity(join => join.ToTable("DepartmentUser"));
        }
    }
}
