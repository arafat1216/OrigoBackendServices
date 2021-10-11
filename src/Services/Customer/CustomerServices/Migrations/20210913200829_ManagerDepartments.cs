using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class ManagerDepartments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Customer_CustomerId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Departments_DepartmentsId",
                table: "DepartmentUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Department");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Department",
                newName: "IX_Department_ParentDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_CustomerId",
                table: "Department",
                newName: "IX_Department_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DepartmentManager",
                columns: table => new
                {
                    ManagersId = table.Column<int>(type: "int", nullable: false),
                    ManagesDepartmentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentManager", x => new { x.ManagersId, x.ManagesDepartmentsId });
                    table.ForeignKey(
                        name: "FK_DepartmentManager_Department_ManagesDepartmentsId",
                        column: x => x.ManagesDepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentManager_User_ManagersId",
                        column: x => x.ManagersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(6488));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 609, DateTimeKind.Utc).AddTicks(9803));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(205));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(207));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(208));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7182));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7386));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7388));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7389));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7429));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 13, 20, 8, 29, 610, DateTimeKind.Utc).AddTicks(7430));

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentManager_ManagesDepartmentsId",
                table: "DepartmentManager",
                column: "ManagesDepartmentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Customer_CustomerId",
                table: "Department",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Department_ParentDepartmentId",
                table: "Department",
                column: "ParentDepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_Department_DepartmentsId",
                table: "DepartmentUser",
                column: "DepartmentsId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Customer_CustomerId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Department_ParentDepartmentId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Department_DepartmentsId",
                table: "DepartmentUser");

            migrationBuilder.DropTable(
                name: "DepartmentManager");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Departments");

            migrationBuilder.RenameIndex(
                name: "IX_Department_ParentDepartmentId",
                table: "Departments",
                newName: "IX_Departments_ParentDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Department_CustomerId",
                table: "Departments",
                newName: "IX_Departments_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Customer_CustomerId",
                table: "Departments",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUser_Departments_DepartmentsId",
                table: "DepartmentUser",
                column: "DepartmentsId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
