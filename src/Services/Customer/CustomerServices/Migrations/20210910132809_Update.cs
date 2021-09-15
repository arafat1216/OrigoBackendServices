using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Departments_UsersId",
                table: "DepartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_User_UsersId1",
                table: "DepartmentUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentUser",
                table: "DepartmentUser");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentUser_UsersId1",
                table: "DepartmentUser");

            migrationBuilder.RenameColumn(
                name: "UsersId1",
                table: "DepartmentUser",
                newName: "DepartmentsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentUser",
                table: "DepartmentUser",
                columns: new[] { "DepartmentsId", "UsersId" });

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(2910));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 240, DateTimeKind.Utc).AddTicks(9937));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 241, DateTimeKind.Utc).AddTicks(1487));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 241, DateTimeKind.Utc).AddTicks(1496));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 241, DateTimeKind.Utc).AddTicks(1498));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(5908));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(6982));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(6984));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(6986));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 10, 13, 28, 8, 243, DateTimeKind.Utc).AddTicks(6988));

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUser_UsersId",
                table: "DepartmentUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_Departments_DepartmentsId",
                table: "DepartmentUser",
                column: "DepartmentsId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_User_UsersId",
                table: "DepartmentUser",
                column: "UsersId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Departments_DepartmentsId",
                table: "DepartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_User_UsersId",
                table: "DepartmentUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentUser",
                table: "DepartmentUser");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentUser_UsersId",
                table: "DepartmentUser");

            migrationBuilder.RenameColumn(
                name: "DepartmentsId",
                table: "DepartmentUser",
                newName: "UsersId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentUser",
                table: "DepartmentUser",
                columns: new[] { "UsersId", "UsersId1" });

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(1900));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 711, DateTimeKind.Utc).AddTicks(528));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 711, DateTimeKind.Utc).AddTicks(2121));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 711, DateTimeKind.Utc).AddTicks(2133));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 711, DateTimeKind.Utc).AddTicks(2135));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4517));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4522));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4523));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4525));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4526));

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUser_UsersId1",
                table: "DepartmentUser",
                column: "UsersId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_Departments_UsersId",
                table: "DepartmentUser",
                column: "UsersId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_User_UsersId1",
                table: "DepartmentUser",
                column: "UsersId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
