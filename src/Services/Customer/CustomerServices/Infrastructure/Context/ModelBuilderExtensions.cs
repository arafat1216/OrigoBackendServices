using System.Reflection;
using Common.Enums;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Context;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // Permissions
        const int CAN_CREATE_CUSTOMER_ID = 1;
        const int CAN_READ_CUSTOMER_ID = 2;
        const int CAN_UPDATE_CUSTOMER_ID = 3;
        const int CAN_DELETE_CUSTOMER_ID = 4;
        const int CAN_READ_ASSET_ID = 5;
        const int CAN_CREATE_ASSET_ID = 6;
        const int CAN_UPDATE_ASSET_ID = 7;
        const int CAN_READ_ONBOARDING_STATUS_ID = 8;
        const int CAN_CREATE_USER_ID = 9;

        modelBuilder.Entity<Permission>().HasData(
            new Permission(CAN_CREATE_CUSTOMER_ID, "CanCreateCustomer", new DateTime(2022, 1, 1)),
            new Permission(CAN_READ_CUSTOMER_ID, "CanReadCustomer", new DateTime(2022, 1, 1)),
            new Permission(CAN_UPDATE_CUSTOMER_ID, "CanUpdateCustomer", new DateTime(2022, 1, 1)),
            new Permission(CAN_DELETE_CUSTOMER_ID, "CanDeleteCustomer", new DateTime(2022, 1, 1)),
            new Permission(CAN_READ_ASSET_ID, "CanReadAsset", new DateTime(2022, 1, 1)),
            new Permission(CAN_CREATE_ASSET_ID, "CanCreateAsset", new DateTime(2022, 1, 1)),
            new Permission(CAN_UPDATE_ASSET_ID, "CanUpdateAsset", new DateTime(2022, 1, 1)),
            new Permission(CAN_READ_ONBOARDING_STATUS_ID, "CanReadOnboardingStatus", new DateTime(2022, 1, 1)),
            new Permission(CAN_CREATE_USER_ID, "CanCreateUser", new DateTime(2022, 1, 1))
        );

        // Roles
        modelBuilder.Entity<Role>().HasData(
            new Role((int) PredefinedRole.EndUser, "EndUser", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.DepartmentManager, "DepartmentManager", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.CustomerAdmin, "CustomerAdmin", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.GroupAdmin, "GroupAdmin", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.PartnerAdmin, "PartnerAdmin", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.PartnerReadOnlyAdmin, "PartnerReadOnlyAdmin", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.SystemAdmin, "SystemAdmin", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.Manager, "Manager", new DateTime(2022, 1, 1)),
            new Role((int)PredefinedRole.Admin, "Admin", new DateTime(2022, 1, 1))
        );

        // PermissionSets
        const int FULL_CUSTOMER_PERMISSION_SET = 1;
        const int INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET = 2;
        const int MANAGER_ACCESS_PERMISSION_SET = 3;
        const int END_USER_ACCESS_PERMISSION_SET = 4;

        modelBuilder.Entity<PermissionSet>().HasData(
            new List<PermissionSet>
            {
                new(FULL_CUSTOMER_PERMISSION_SET, "FullCustomerAccess", new DateTime(2022, 1, 1)),
                new(INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, "InternalCustomerAccess", new DateTime(2022, 1, 1)),
                new(MANAGER_ACCESS_PERMISSION_SET, "ManagerAccess", new DateTime(2022, 1, 1)),
                new(END_USER_ACCESS_PERMISSION_SET, "EndUserAccess", new DateTime(2022, 1, 1))
            });

        modelBuilder.Entity<PermissionSet>()
            .HasMany(p => p.Permissions)
            .WithMany(p => p.PermissionSets)
            .UsingEntity(ps => ps.HasData(
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_CREATE_CUSTOMER_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_READ_CUSTOMER_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_UPDATE_CUSTOMER_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_DELETE_CUSTOMER_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_READ_ASSET_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_CREATE_ASSET_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_UPDATE_ASSET_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_READ_ONBOARDING_STATUS_ID },
                new { PermissionSetsId = FULL_CUSTOMER_PERMISSION_SET, PermissionsId = CAN_CREATE_USER_ID },

                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_CUSTOMER_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_UPDATE_CUSTOMER_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_ASSET_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_CREATE_ASSET_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_UPDATE_ASSET_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_ONBOARDING_STATUS_ID },
                new { PermissionSetsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, PermissionsId = CAN_CREATE_USER_ID },

                new { PermissionSetsId = MANAGER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_CUSTOMER_ID },
                new { PermissionSetsId = MANAGER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_ASSET_ID },
                new { PermissionSetsId = MANAGER_ACCESS_PERMISSION_SET, PermissionsId = CAN_UPDATE_ASSET_ID },

                new { PermissionSetsId = END_USER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_CUSTOMER_ID },
                new { PermissionSetsId = END_USER_ACCESS_PERMISSION_SET, PermissionsId = CAN_READ_ASSET_ID },
                new { PermissionSetsId = END_USER_ACCESS_PERMISSION_SET, PermissionsId = CAN_UPDATE_ASSET_ID }
            ));

        modelBuilder.Entity<PermissionSet>().HasMany(p => p.Roles).WithMany(r => r.GrantedPermissions).UsingEntity(pr =>
            pr.HasData(
                new { GrantedPermissionsId = FULL_CUSTOMER_PERMISSION_SET, RolesId = (int) PredefinedRole.SystemAdmin },
                new { GrantedPermissionsId = FULL_CUSTOMER_PERMISSION_SET, RolesId = (int) PredefinedRole.PartnerAdmin },
                new { GrantedPermissionsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, RolesId = (int) PredefinedRole.CustomerAdmin },
                new { GrantedPermissionsId = INTERNAL_CUSTOMER_ACCESS_PERMISSION_SET, RolesId = (int) PredefinedRole.Admin },
                new { GrantedPermissionsId = MANAGER_ACCESS_PERMISSION_SET, RolesId = (int) PredefinedRole.DepartmentManager },
                new { GrantedPermissionsId = MANAGER_ACCESS_PERMISSION_SET, RolesId = (int) PredefinedRole.Manager },
                new { GrantedPermissionsId = END_USER_ACCESS_PERMISSION_SET, RolesId = (int) PredefinedRole.EndUser })
            );
    }
}
