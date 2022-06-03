using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class fixed_default_values : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Owner_UserId",
                table: "HardwareServiceOrders",
                defaultValue: null);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "HardwareServiceOrders",
                defaultValue: null);

            migrationBuilder.AlterColumn<string>(
                name: "Owner_FirstName",
                table: "HardwareServiceOrders",
                defaultValue: null);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceProviderOrderId1",
                table: "HardwareServiceOrders",
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Owner_UserId",
                table: "HardwareServiceOrders",
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "HardwareServiceOrders",
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Owner_FirstName",
                table: "HardwareServiceOrders",
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceProviderOrderId1",
                table: "HardwareServiceOrders",
                defaultValue: "");
        }
    }
}
