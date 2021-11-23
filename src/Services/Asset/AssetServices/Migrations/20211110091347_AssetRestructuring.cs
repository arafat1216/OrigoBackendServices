using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class AssetRestructuring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asset",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "UsesImei",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "Asset");

            //migrationBuilder.DropColumn(
            //    name: "IsActive",
            //    table: "Asset");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Asset");

            migrationBuilder.RenameTable(
                name: "Asset",
                newName: "Assets");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Assets",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "MacAddress",
                table: "Assets",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Imei",
                table: "Assets",
                newName: "AssetTag");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "Assets",
                newName: "ExternalId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assets",
                table: "Assets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AssetCategoryTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetCategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategoryTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetCategoryTranslation_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HardwareType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardwareType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HardwareType_Assets_Id",
                        column: x => x.Id,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SerialKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftwareType_Assets_Id",
                        column: x => x.Id,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetImei",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imei = table.Column<long>(type: "bigint", nullable: false),
                    HardwareSuperTypeId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetImei", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetImei_HardwareType_HardwareSuperTypeId",
                        column: x => x.HardwareSuperTypeId,
                        principalTable: "HardwareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MobilePhone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobilePhone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobilePhone_HardwareType_Id",
                        column: x => x.Id,
                        principalTable: "HardwareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tablet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tablet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tablet_HardwareType_Id",
                        column: x => x.Id,
                        principalTable: "HardwareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_SoftwareType_Id",
                        column: x => x.Id,
                        principalTable: "SoftwareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryTranslation_AssetCategoryId",
                table: "AssetCategoryTranslation",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetImei_HardwareSuperTypeId",
                table: "AssetImei",
                column: "HardwareSuperTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetCategoryTranslation");

            migrationBuilder.DropTable(
                name: "AssetImei");

            migrationBuilder.DropTable(
                name: "MobilePhone");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "Tablet");

            migrationBuilder.DropTable(
                name: "SoftwareType");

            migrationBuilder.DropTable(
                name: "HardwareType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assets",
                table: "Assets");

            migrationBuilder.RenameTable(
                name: "Assets",
                newName: "Asset");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Asset",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Asset",
                newName: "AssetId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Asset",
                newName: "MacAddress");

            migrationBuilder.RenameColumn(
                name: "AssetTag",
                table: "Asset",
                newName: "Imei");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetCategoryId",
                table: "AssetCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AssetCategory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UsesImei",
                table: "AssetCategory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "Asset",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsActive",
            //    table: "Asset",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Asset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asset",
                table: "Asset",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
