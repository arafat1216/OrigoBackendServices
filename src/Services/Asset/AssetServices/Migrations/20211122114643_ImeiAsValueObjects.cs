using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class ImeiAsValueObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetImei",
                table: "AssetImei");

            migrationBuilder.DropIndex(
                name: "IX_AssetImei_HardwareAssetId",
                table: "AssetImei");

            migrationBuilder.AlterColumn<int>(
                name: "HardwareAssetId",
                table: "AssetImei",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetImei",
                table: "AssetImei",
                columns: new[] { "HardwareAssetId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei",
                column: "HardwareAssetId",
                principalTable: "HardwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetImei",
                table: "AssetImei");

            migrationBuilder.AlterColumn<int>(
                name: "HardwareAssetId",
                table: "AssetImei",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetImei",
                table: "AssetImei",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetImei_HardwareAssetId",
                table: "AssetImei",
                column: "HardwareAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetImei_HardwareAsset_HardwareAssetId",
                table: "AssetImei",
                column: "HardwareAssetId",
                principalTable: "HardwareAsset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
