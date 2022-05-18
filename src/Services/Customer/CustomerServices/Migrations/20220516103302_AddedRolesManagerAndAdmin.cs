using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class AddedRolesManagerAndAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6740));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6633));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6635));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6635));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6636));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6752));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6753));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6755));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6755));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 8, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6756), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manager", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 5, 16, 10, 33, 1, 717, DateTimeKind.Utc).AddTicks(6756), null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6000));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(5913));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(5915));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(5916));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(5916));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6063));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6064));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6066));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2022, 4, 11, 13, 44, 26, 306, DateTimeKind.Utc).AddTicks(6066));
        }
    }
}
