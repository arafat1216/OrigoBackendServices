using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class permissionSetsSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PermissionPermissionSet", true);
            migrationBuilder.Sql("DELETE FROM PermissionSets", true);
            migrationBuilder.Sql("DELETE FROM PermissionSetRole", true);
            migrationBuilder.Sql("DELETE FROM [Permissions]", true);

            migrationBuilder.InsertData(
                table: "PermissionSets",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "FullCustomerAccess", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "InternalCustomerAccess", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ManagerAccess", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "EndUserAccess", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanCreateCustomer", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanReadCustomer", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanUpdateCustomer", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanDeleteCustomer", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanReadAsset", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanCreateAsset", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanUpdateAsset", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanReadOnboardingStatus", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "PermissionPermissionSet",
                columns: new[] { "PermissionSetsId", "PermissionsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 8 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 7 },
                    { 2, 8 },
                    { 3, 2 },
                    { 3, 5 },
                    { 3, 7 },
                    { 4, 2 },
                    { 4, 5 },
                    { 4, 7 }
                });

            migrationBuilder.InsertData(
                table: "PermissionSetRole",
                columns: new[] { "GrantedPermissionsId", "RolesId" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 1, 6 },
                    { 2, 3 },
                    { 2, 9 },
                    { 3, 2 },
                    { 3, 8 },
                    { 4, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 7 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 8 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 7 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 8 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 3, 7 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 2, 9 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 3, 8 });

            migrationBuilder.DeleteData(
                table: "PermissionSetRole",
                keyColumns: new[] { "GrantedPermissionsId", "RolesId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
