using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class SalesforceTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesforceTicketId",
                table: "TransferToPrivateSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesforceTicketId",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesforceTicketId",
                table: "ChangeSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesforceTicketId",
                table: "TransferToPrivateSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SalesforceTicketId",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SalesforceTicketId",
                table: "ChangeSubscriptionOrder");
        }
    }
}
