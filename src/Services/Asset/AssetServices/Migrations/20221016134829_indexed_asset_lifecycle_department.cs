using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class indexed_asset_lifecycle_department : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AssetLifeCycles_ManagedByDepartmentId",
                table: "AssetLifeCycles",
                column: "ManagedByDepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetLifeCycles_ManagedByDepartmentId",
                table: "AssetLifeCycles");
        }
    }
}
