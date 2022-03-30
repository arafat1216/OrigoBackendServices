using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class addedSimCardAddress_TransferToBusinessSubscriptionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SimCardAddress",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimCardCountry",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimCardPostalCode",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimCardPostalPlace",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimCardReciverFirstName",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimCardReciverLastName",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimCardAddress",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SimCardCountry",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SimCardPostalCode",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SimCardPostalPlace",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SimCardReciverFirstName",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SimCardReciverLastName",
                table: "TransferToBusinessSubscriptionOrder");
        }
    }
}
