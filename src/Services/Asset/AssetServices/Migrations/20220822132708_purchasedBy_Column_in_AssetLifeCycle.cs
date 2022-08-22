using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class purchasedBy_Column_in_AssetLifeCycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisposeSetting_PayrollContactEmail",
                table: "CustomerSettings");

            migrationBuilder.AddColumn<string>(
                name: "PurchasedBy",
                table: "AssetLifeCycles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasedBy",
                table: "AssetLifeCycles");

            migrationBuilder.AddColumn<string>(
                name: "DisposeSetting_PayrollContactEmail",
                table: "CustomerSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
