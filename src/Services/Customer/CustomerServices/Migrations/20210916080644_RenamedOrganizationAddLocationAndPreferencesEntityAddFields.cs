using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class RenamedOrganizationAddLocationAndPreferencesEntityAddFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
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
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
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

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(2845));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(3760));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4473));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4475));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4476));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(3841));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4187));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4190));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4191));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4193));

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategory_Customer_OrganizationId",
                table: "AssetCategory",
                column: "OrganizationId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_Customer_OrganizationId",
                table: "AssetCategory");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "OrganizationPreferences");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
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
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
