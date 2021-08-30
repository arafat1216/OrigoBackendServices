﻿using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
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
        }
    }
}
