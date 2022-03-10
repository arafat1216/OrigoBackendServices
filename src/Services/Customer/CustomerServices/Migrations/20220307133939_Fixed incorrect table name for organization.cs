using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class Fixedincorrecttablenamefororganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Customer_CustomerId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_Customer_OrganizationId",
                table: "Partner");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Customer_CustomerId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Organization");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9585));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9428));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9431));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9431));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9432));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9612));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9613));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9615));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 7, 13, 39, 39, 246, DateTimeKind.Utc).AddTicks(9616));

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Organization_CustomerId",
                table: "Department",
                column: "CustomerId",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_Organization_OrganizationId",
                table: "Partner",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Organization_CustomerId",
                table: "User",
                column: "CustomerId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Organization_CustomerId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_Organization_OrganizationId",
                table: "Partner");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Organization_CustomerId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Customer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9742));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9577));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9580));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9581));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9582));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9765));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9766));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9766));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9767));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9768));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 14, 59, 50, 40, DateTimeKind.Utc).AddTicks(9768));

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Customer_CustomerId",
                table: "Department",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_Customer_OrganizationId",
                table: "Partner",
                column: "OrganizationId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Customer_CustomerId",
                table: "User",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }
    }
}
