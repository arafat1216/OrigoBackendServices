using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class AssetCategoryParents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentAssetCategoryId",
                table: "AssetCategory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategory_ParentAssetCategoryId",
                table: "AssetCategory",
                column: "ParentAssetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategory_AssetCategory_ParentAssetCategoryId",
                table: "AssetCategory",
                column: "ParentAssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_AssetCategory_ParentAssetCategoryId",
                table: "AssetCategory");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategory_ParentAssetCategoryId",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "ParentAssetCategoryId",
                table: "AssetCategory");
        }
    }
}
