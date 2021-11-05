using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class RenameFieldFullName_toFirstName_add_field_LastName_to_ContactPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactPerson_FullName",
                table: "Customer",
                newName: "ContactPerson_LastName");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson_FirstName",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 296, DateTimeKind.Utc).AddTicks(9989));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(6905));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7927));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7931));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7933));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1405));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1901));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1905));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1906));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1907));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1908));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson_FirstName",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "ContactPerson_LastName",
                table: "Customer",
                newName: "ContactPerson_FullName");

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(7033));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(3302));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4346));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4347));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(8576));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9091));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9095));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9096));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9097));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9098));
        }
    }
}
