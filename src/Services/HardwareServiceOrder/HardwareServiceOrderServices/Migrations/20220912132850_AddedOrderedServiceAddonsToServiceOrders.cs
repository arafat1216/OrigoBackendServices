using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class AddedOrderedServiceAddonsToServiceOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncludedServiceOrderAddonIds",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServiceOrderAddons",
                keyColumn: "Id",
                keyValue: 1,
                column: "ThirdPartyId",
                value: "268");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludedServiceOrderAddonIds",
                table: "HardwareServiceOrders");

            migrationBuilder.UpdateData(
                table: "ServiceOrderAddons",
                keyColumn: "Id",
                keyValue: 1,
                column: "ThirdPartyId",
                value: "");
        }
    }
}
