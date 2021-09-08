using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class Departments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUser",
                columns: table => new
                {
                    UsersId = table.Column<int>(type: "int", nullable: false),
                    UsersId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUser", x => new { x.UsersId, x.UsersId1 });
                    table.ForeignKey(
                        name: "FK_DepartmentUser_Departments_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentUser_User_UsersId1",
                        column: x => x.UsersId1,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "LastUpdatedDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(3895), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "EndUser" },
                    { 2, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4517), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DepartmentManager" },
                    { 3, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4522), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CustomerAdmin" },
                    { 4, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4523), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "GroupAdmin" },
                    { 5, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4525), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PartnerAdmin" },
                    { 6, new DateTime(2021, 9, 6, 6, 59, 22, 713, DateTimeKind.Utc).AddTicks(4526), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SystemAdmin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CustomerId",
                table: "Departments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUser_UsersId1",
                table: "DepartmentUser",
                column: "UsersId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentUser");

            migrationBuilder.DropTable(
                name: "Departments");

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
