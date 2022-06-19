using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class reworked_service_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetInfo_Brand",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetInfo_Model",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssetInfo_PurchaseDate",
                table: "HardwareServiceOrders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetInfo_SerialNumber",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetLifecycleCategoryId",
                table: "HardwareServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Owner_LastName",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Owner_PhoneNumber",
                table: "HardwareServiceOrders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedAssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedAssetInfo_Brand",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedAssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedAssetInfo_Model",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedAssetInfo_PurchaseDate",
                table: "HardwareServiceOrders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedAssetInfo_SerialNumber",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetInfo_Accessories",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetInfo_Brand",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetInfo_Imei",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetInfo_Model",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetInfo_PurchaseDate",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetInfo_SerialNumber",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "AssetLifecycleCategoryId",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "Owner_LastName",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "Owner_PhoneNumber",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_Accessories",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_Brand",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_Imei",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_Model",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_PurchaseDate",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ReturnedAssetInfo_SerialNumber",
                table: "HardwareServiceOrders");
        }
    }
}
