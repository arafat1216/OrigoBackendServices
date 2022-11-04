using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class UserPermission_CanCreateUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "Name", "UpdatedBy" },
                values: new object[] { 9, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanCreateUser", new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "PermissionPermissionSet",
                columns: new[] { "PermissionSetsId", "PermissionsId" },
                values: new object[] { 1, 9 });

            migrationBuilder.InsertData(
                table: "PermissionPermissionSet",
                columns: new[] { "PermissionSetsId", "PermissionsId" },
                values: new object[] { 2, 9 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "PermissionPermissionSet",
                keyColumns: new[] { "PermissionSetsId", "PermissionsId" },
                keyValues: new object[] { 2, 9 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
