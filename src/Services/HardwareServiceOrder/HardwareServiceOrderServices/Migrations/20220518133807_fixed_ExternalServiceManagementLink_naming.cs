using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class fixed_ExternalServiceManagementLink_naming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalProviderLink",
                table: "HardwareServiceOrders",
                newName: "ServiceProviderOrderId2");

            migrationBuilder.AddColumn<string>(
                name: "ExternalServiceManagementLink",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceProviderOrderId1",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalServiceManagementLink",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "ServiceProviderOrderId1",
                table: "HardwareServiceOrders");

            migrationBuilder.RenameColumn(
                name: "ServiceProviderOrderId2",
                table: "HardwareServiceOrders",
                newName: "ExternalProviderLink");
        }
    }
}
