using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class rename_asset_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetImei_HardwareType_HardwareSuperTypeId",
                table: "AssetImei");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetCategory_AssetCategoryId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_HardwareType_Assets_Id",
                table: "HardwareType");

            migrationBuilder.DropForeignKey(
                name: "FK_MobilePhone_HardwareType_Id",
                table: "MobilePhone");

            migrationBuilder.DropForeignKey(
                name: "FK_SoftwareType_Assets_Id",
                table: "SoftwareType");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_SoftwareType_Id",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Tablet_HardwareType_Id",
                table: "Tablet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assets",
                table: "Assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoftwareType",
                table: "SoftwareType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HardwareType",
                table: "HardwareType");

            migrationBuilder.RenameTable(
                name: "Assets",
                newName: "Asset");

            migrationBuilder.RenameTable(
                name: "SoftwareType",
                newName: "SoftwareAsset");

            migrationBuilder.RenameTable(
                name: "HardwareType",
                newName: "HardwareAsset");

            migrationBuilder.RenameColumn(
                name: "HardwareSuperTypeId",
                table: "AssetImei",
                newName: "HardwareAssetId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetImei_HardwareSuperTypeId",
                table: "AssetImei",
                newName: "IX_AssetImei_HardwareAssetId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetCategoryId",
                table: "Asset",
                newName: "IX_Asset_AssetCategoryId");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetImei",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "AssetImei",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AssetImei",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AssetImei",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetCategoryTranslation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "AssetCategoryTranslation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AssetCategoryTranslation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AssetCategoryTranslation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asset",
                table: "Asset",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoftwareAsset",
                table: "SoftwareAsset",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HardwareAsset",
                table: "HardwareAsset",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei",
                column: "HardwareAssetId",
                principalTable: "HardwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HardwareAsset_Asset_Id",
                table: "HardwareAsset",
                column: "Id",
                principalTable: "Asset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MobilePhone_HardwareAsset_Id",
                table: "MobilePhone",
                column: "Id",
                principalTable: "HardwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareAsset_Asset_Id",
                table: "SoftwareAsset",
                column: "Id",
                principalTable: "Asset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_SoftwareAsset_Id",
                table: "Subscription",
                column: "Id",
                principalTable: "SoftwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tablet_HardwareAsset_Id",
                table: "Tablet",
                column: "Id",
                principalTable: "HardwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei");

            migrationBuilder.DropForeignKey(
                name: "FK_HardwareAsset_Asset_Id",
                table: "HardwareAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_MobilePhone_HardwareAsset_Id",
                table: "MobilePhone");

            migrationBuilder.DropForeignKey(
                name: "FK_SoftwareAsset_Asset_Id",
                table: "SoftwareAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_SoftwareAsset_Id",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Tablet_HardwareAsset_Id",
                table: "Tablet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asset",
                table: "Asset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoftwareAsset",
                table: "SoftwareAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HardwareAsset",
                table: "HardwareAsset");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetImei");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AssetImei");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AssetImei");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetImei");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetCategoryTranslation");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AssetCategoryTranslation");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AssetCategoryTranslation");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetCategoryTranslation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Asset");

            migrationBuilder.RenameTable(
                name: "Asset",
                newName: "Assets");

            migrationBuilder.RenameTable(
                name: "SoftwareAsset",
                newName: "SoftwareType");

            migrationBuilder.RenameTable(
                name: "HardwareAsset",
                newName: "HardwareType");

            migrationBuilder.RenameColumn(
                name: "HardwareAssetId",
                table: "AssetImei",
                newName: "HardwareSuperTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetImei_HardwareAssetId",
                table: "AssetImei",
                newName: "IX_AssetImei_HardwareSuperTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Assets",
                newName: "IX_Assets_AssetCategoryId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoftwareType",
                table: "SoftwareType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HardwareType",
                table: "HardwareType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetImei_HardwareType_HardwareSuperTypeId",
                table: "AssetImei",
                column: "HardwareSuperTypeId",
                principalTable: "HardwareType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetCategory_AssetCategoryId",
                table: "Assets",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HardwareType_Assets_Id",
                table: "HardwareType",
                column: "Id",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MobilePhone_HardwareType_Id",
                table: "MobilePhone",
                column: "Id",
                principalTable: "HardwareType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareType_Assets_Id",
                table: "SoftwareType",
                column: "Id",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_SoftwareType_Id",
                table: "Subscription",
                column: "Id",
                principalTable: "SoftwareType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tablet_HardwareType_Id",
                table: "Tablet",
                column: "Id",
                principalTable: "HardwareType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
