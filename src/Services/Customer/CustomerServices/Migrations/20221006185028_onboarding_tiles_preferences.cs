using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class onboarding_tiles_preferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAssetTileClosed",
                table: "UserPreference",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscriptionTileClosed",
                table: "UserPreference",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssetTileClosed",
                table: "UserPreference");

            migrationBuilder.DropColumn(
                name: "IsSubscriptionTileClosed",
                table: "UserPreference");
        }
    }
}
