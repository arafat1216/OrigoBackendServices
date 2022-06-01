using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CustomerServices.Infrastructure.Context
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(
                new Permission(1, "CanCreateCustomer", new DateTime(2022, 1, 1)),
                new Permission(2, "CanReadCustomer", new DateTime(2022, 1, 1)),
                new Permission(3, "CanUpdateCustomer", new DateTime(2022, 1, 1)),
                new Permission(4, "CanDeleteCustomer", new DateTime(2022, 1, 1))
            );
            
            modelBuilder.Entity<PermissionSet>().HasData(
                new PermissionSet(1, "FullCustomerAccess", new DateTime(2022, 1, 1))
            );

            modelBuilder.Entity<Role>().HasData(
                new Role(1, "EndUser", new DateTime(2022, 1, 1)),
                new Role(2, "DepartmentManager", new DateTime(2022, 1, 1)),
                new Role(3, "CustomerAdmin", new DateTime(2022, 1, 1)),
                new Role(4, "GroupAdmin", new DateTime(2022, 1, 1)),
                new Role(5, "PartnerAdmin", new DateTime(2022, 1, 1)),
                new Role(7, "PartnerReadOnlyAdmin", new DateTime(2022, 1, 1)),
                new Role(6, "SystemAdmin", new DateTime(2022, 1, 1)),
                new Role(8, "Manager", new DateTime(2022, 1, 1)),
                new Role(9, "Admin", new DateTime(2022, 1, 1))
            );
        }
    }
}