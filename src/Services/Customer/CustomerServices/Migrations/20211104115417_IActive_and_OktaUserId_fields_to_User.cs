using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class IActive_and_OktaUserId_fields_to_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OktaUserId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 882, DateTimeKind.Utc).AddTicks(9149));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 881, DateTimeKind.Utc).AddTicks(7343));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 881, DateTimeKind.Utc).AddTicks(8261));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 881, DateTimeKind.Utc).AddTicks(8264));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 881, DateTimeKind.Utc).AddTicks(8266));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(1002));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(1005));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(1007));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(1008));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 11, 4, 11, 54, 16, 883, DateTimeKind.Utc).AddTicks(1009));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "User");

            migrationBuilder.DropColumn(
                name: "OktaUserId",
                table: "User");

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
