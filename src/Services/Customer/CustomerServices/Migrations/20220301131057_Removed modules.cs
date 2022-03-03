using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class Removedmodules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerProductModule");

            migrationBuilder.DropTable(
                name: "CustomerProductModuleGroup");

            migrationBuilder.DropTable(
                name: "ProductModuleGroups");

            migrationBuilder.DropTable(
                name: "ProductModule");

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9204));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(8997));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(8999));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9000));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9001));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9246));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9248));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9249));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9249));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9250));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 3, 1, 13, 10, 57, 320, DateTimeKind.Utc).AddTicks(9250));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerProductModule",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModulesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductModule", x => new { x.CustomersId, x.SelectedProductModulesId });
                    table.ForeignKey(
                        name: "FK_CustomerProductModule_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerProductModule_ProductModule_SelectedProductModulesId",
                        column: x => x.SelectedProductModulesId,
                        principalTable: "ProductModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductModuleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductModuleExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductModuleGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductModuleId = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModuleGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductModuleGroups_ProductModule_ProductModuleId",
                        column: x => x.ProductModuleId,
                        principalTable: "ProductModule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerProductModuleGroup",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModuleGroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductModuleGroup", x => new { x.CustomersId, x.SelectedProductModuleGroupsId });
                    table.ForeignKey(
                        name: "FK_CustomerProductModuleGroup_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerProductModuleGroup_ProductModuleGroups_SelectedProductModuleGroupsId",
                        column: x => x.SelectedProductModuleGroupsId,
                        principalTable: "ProductModuleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 256, DateTimeKind.Utc).AddTicks(9875));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 256, DateTimeKind.Utc).AddTicks(3462));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 256, DateTimeKind.Utc).AddTicks(3856));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 256, DateTimeKind.Utc).AddTicks(3858));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 256, DateTimeKind.Utc).AddTicks(3858));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(539));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(735));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(737));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(737));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(738));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2022, 2, 18, 12, 37, 24, 257, DateTimeKind.Utc).AddTicks(739));

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModule_SelectedProductModulesId",
                table: "CustomerProductModule",
                column: "SelectedProductModulesId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModuleGroup_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup",
                column: "SelectedProductModuleGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductModuleGroups_ProductModuleId",
                table: "ProductModuleGroups",
                column: "ProductModuleId");
        }
    }
}
