using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class remove_asset_category_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerServiceProvider",
                table: "CustomerServiceProvider");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "CustomerServiceProvider");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerServiceProvider",
                table: "CustomerServiceProvider",
                columns: new[] { "CustomerId", "Id", "ServiceProviderId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerServiceProvider",
                table: "CustomerServiceProvider");

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "CustomerServiceProvider",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerServiceProvider",
                table: "CustomerServiceProvider",
                columns: new[] { "CustomerId", "Id", "AssetCategoryId", "ServiceProviderId" });
        }
    }
}
