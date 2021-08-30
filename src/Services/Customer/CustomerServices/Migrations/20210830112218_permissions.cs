using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionPermissionSet",
                columns: table => new
                {
                    PermissionSetsId = table.Column<int>(type: "int", nullable: false),
                    PermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionPermissionSet", x => new { x.PermissionSetsId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_PermissionPermissionSet_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionPermissionSet_PermissionSets_PermissionSetsId",
                        column: x => x.PermissionSetsId,
                        principalTable: "PermissionSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionSetRole",
                columns: table => new
                {
                    GrantedPermissionsId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionSetRole", x => new { x.GrantedPermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_PermissionSetRole_PermissionSets_GrantedPermissionsId",
                        column: x => x.GrantedPermissionsId,
                        principalTable: "PermissionSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionSetRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    AccessList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PermissionSets",
                columns: new[] { "Id", "CreatedDate", "LastUpdatedDate", "Name" },
                values: new object[] { 1, new DateTime(2021, 8, 30, 11, 22, 18, 654, DateTimeKind.Utc).AddTicks(2402), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "FullCustomerAccess" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedDate", "LastUpdatedDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5553), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanCreateCustomer" },
                    { 2, new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5958), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanReadCustomer" },
                    { 3, new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5960), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanUpdateCustomer" },
                    { 4, new DateTime(2021, 8, 30, 11, 22, 18, 653, DateTimeKind.Utc).AddTicks(5961), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CanDeleteCustomer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionPermissionSet_PermissionsId",
                table: "PermissionPermissionSet",
                column: "PermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionSetRole_RolesId",
                table: "PermissionSetRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_RoleId",
                table: "UserPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionPermissionSet");

            migrationBuilder.DropTable(
                name: "PermissionSetRole");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PermissionSets");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
