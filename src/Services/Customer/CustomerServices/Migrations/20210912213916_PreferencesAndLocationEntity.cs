using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class PreferencesAndLocationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Department_ParentDepartmentId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Department_UsersId1",
                table: "DepartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_User_UsersId",
                table: "DepartmentUser");

            migrationBuilder.DropTable(
                name: "CustomerProductModule");

            migrationBuilder.DropTable(
                name: "CustomerProductModuleGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Departments");

            migrationBuilder.RenameColumn(
                name: "CustomerContactPerson_PhoneNumber",
                table: "Customer",
                newName: "OrganizationContactPerson_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerContactPerson_FullName",
                table: "Customer",
                newName: "OrganizationContactPerson_FullName");

            migrationBuilder.RenameColumn(
                name: "CustomerContactPerson_Email",
                table: "Customer",
                newName: "OrganizationContactPerson_Email");

            migrationBuilder.RenameColumn(
                name: "CompanyAddress_Street",
                table: "Customer",
                newName: "OrganizationAddress_Street");

            migrationBuilder.RenameColumn(
                name: "CompanyAddress_PostCode",
                table: "Customer",
                newName: "OrganizationAddress_PostCode");

            migrationBuilder.RenameColumn(
                name: "CompanyAddress_Country",
                table: "Customer",
                newName: "OrganizationAddress_Country");

            migrationBuilder.RenameColumn(
                name: "CompanyAddress_City",
                table: "Customer",
                newName: "OrganizationAddress_City");

            migrationBuilder.RenameColumn(
                name: "OrgNumber",
                table: "Customer",
                newName: "OrganizationNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customer",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Customer",
                newName: "OrganizationName");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "AssetCategory",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCategory_CustomerId",
                table: "AssetCategory",
                newName: "IX_AssetCategory_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Department_ParentDepartmentId",
                table: "Departments",
                newName: "IX_Departments_ParentDepartmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryLocation",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalDepartmentId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebPage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnforceTwoFactorAuth = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultDepartmentClassification = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProductModule",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModulesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProductModule", x => new { x.CustomersId, x.SelectedProductModulesId });
                    table.ForeignKey(
                        name: "FK_OrganizationProductModule_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProductModule_ProductModule_SelectedProductModulesId",
                        column: x => x.SelectedProductModulesId,
                        principalTable: "ProductModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProductModuleGroup",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModuleGroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProductModuleGroup", x => new { x.CustomersId, x.SelectedProductModuleGroupsId });
                    table.ForeignKey(
                        name: "FK_OrganizationProductModuleGroup_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProductModuleGroup_ProductModuleGroups_SelectedProductModuleGroupsId",
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
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(6708));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 795, DateTimeKind.Utc).AddTicks(5524));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 795, DateTimeKind.Utc).AddTicks(6425));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 795, DateTimeKind.Utc).AddTicks(6429));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 795, DateTimeKind.Utc).AddTicks(6431));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(7985));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(8480));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(8483));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(8484));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(8485));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 12, 21, 39, 15, 796, DateTimeKind.Utc).AddTicks(8487));

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CustomerId",
                table: "Departments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProductModule_SelectedProductModulesId",
                table: "OrganizationProductModule",
                column: "SelectedProductModulesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProductModuleGroup_SelectedProductModuleGroupsId",
                table: "OrganizationProductModuleGroup",
                column: "SelectedProductModuleGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategory_Customer_OrganizationId",
                table: "AssetCategory",
                column: "OrganizationId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_Customer_OrganizationId",
                table: "AssetCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Customer_CustomerId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_Departments_UsersId",
                table: "DepartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUser_User_UsersId1",
                table: "DepartmentUser");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "OrganizationPreferences");

            migrationBuilder.DropTable(
                name: "OrganizationProductModule");

            migrationBuilder.DropTable(
                name: "OrganizationProductModuleGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CustomerId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PrimaryLocation",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ExternalDepartmentId",
                table: "Departments");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_PhoneNumber",
                table: "Customer",
                newName: "CustomerContactPerson_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_FullName",
                table: "Customer",
                newName: "CustomerContactPerson_FullName");

            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_Email",
                table: "Customer",
                newName: "CustomerContactPerson_Email");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_Street",
                table: "Customer",
                newName: "CompanyAddress_Street");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_PostCode",
                table: "Customer",
                newName: "CompanyAddress_PostCode");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_Country",
                table: "Customer",
                newName: "CompanyAddress_Country");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_City",
                table: "Customer",
                newName: "CompanyAddress_City");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Customer",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "OrganizationNumber",
                table: "Customer",
                newName: "OrgNumber");

            migrationBuilder.RenameColumn(
                name: "OrganizationName",
                table: "Customer",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "AssetCategory",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCategory_OrganizationId",
                table: "AssetCategory",
                newName: "IX_AssetCategory_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Department",
                newName: "IX_Department_ParentDepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1173));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1388));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1388));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1389));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 7, 6, 27, 55, 583, DateTimeKind.Utc).AddTicks(1390));

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModule_SelectedProductModulesId",
                table: "CustomerProductModule",
                column: "SelectedProductModulesId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModuleGroup_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup",
                column: "SelectedProductModuleGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory",
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
                name: "FK_DepartmentUser_Department_UsersId1",
                table: "DepartmentUser",
                column: "UsersId1",
                principalTable: "Department",
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
    }
}
