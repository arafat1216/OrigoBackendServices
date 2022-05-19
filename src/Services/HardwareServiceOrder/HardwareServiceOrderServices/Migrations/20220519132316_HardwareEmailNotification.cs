using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class HardwareEmailNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOrderDiscardedEmailSent",
                table: "HardwareServiceOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturnLoanDeviceEmailSent",
                table: "HardwareServiceOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrderDiscardedEmailSent",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "IsReturnLoanDeviceEmailSent",
                table: "HardwareServiceOrders");
        }
    }
}
