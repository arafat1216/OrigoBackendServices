using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class AssetCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategoryLifecycleType_Customer_CustomerId1",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategoryLifecycleType_CustomerId1",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "AssetCategoryLifecycleType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "AssetCategoryLifecycleType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryLifecycleType_CustomerId1",
                table: "AssetCategoryLifecycleType",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategoryLifecycleType_Customer_CustomerId1",
                table: "AssetCategoryLifecycleType",
                column: "CustomerId1",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
