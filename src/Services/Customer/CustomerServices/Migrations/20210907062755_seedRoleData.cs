using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class seedRoleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(366));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 582, DateTimeKind.Utc).AddTicks(3358));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 582, DateTimeKind.Utc).AddTicks(3864));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 582, DateTimeKind.Utc).AddTicks(3867));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 582, DateTimeKind.Utc).AddTicks(3868));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "LastUpdatedDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1173), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "EndUser" },
                    { 2, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1386), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DepartmentManager" },
                    { 3, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1388), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CustomerAdmin" },
                    { 4, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1388), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "GroupAdmin" },
                    { 5, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1389), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PartnerAdmin" },
                    { 6, new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1390), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SystemAdmin" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentUser");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 8, 30, 11, 22, 18, 654, DateTimeKind.Utc).AddTicks(2402));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5553));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5958));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5960));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5961));
        }
    }
}
