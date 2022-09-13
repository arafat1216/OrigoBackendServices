using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class AddedCustomerSettingsAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSettings_CustomerId",
                table: "CustomerSettings");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CustomerSettings_CustomerId",
                table: "CustomerSettings",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CustomerSettings_CustomerId",
                table: "CustomerSettings");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSettings_CustomerId",
                table: "CustomerSettings",
                column: "CustomerId");
        }
    }
}
