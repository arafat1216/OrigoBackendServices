using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Context
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(
                new Permission(1, "CanCreateCustomer"),
                new Permission(2, "CanReadCustomer"),
                new Permission(3, "CanUpdateCustomer"),
                new Permission(4, "CanDeleteCustomer")
            );
            
            modelBuilder.Entity<PermissionSet>().HasData(
                new PermissionSet(1, "FullCustomerAccess")
            );

            modelBuilder.Entity<Role>().HasData(
                new Role(1, "EndUser"),
                new Role(2, "DepartmentManager"),
                new Role(3, "CustomerAdmin"),
                new Role(4, "GroupAdmin"),
                new Role(5, "PartnerAdmin"),
                new Role(7, "PartnerReadOnlyAdmin"),
                new Role(6, "SystemAdmin"),
                new Role(8, "Manager"),
                new Role(9, "Admin")
            );
        }
    }
}