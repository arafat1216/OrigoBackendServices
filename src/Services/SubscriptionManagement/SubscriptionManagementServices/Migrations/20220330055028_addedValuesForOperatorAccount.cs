using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class addedValuesForOperatorAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SIMCardAction",
                table: "TransferToBusinessSubscriptionOrder",
                newName: "SimCardAction");

            migrationBuilder.AddColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumberOwner",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumberPayer",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "NewSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumberOwner",
                table: "NewSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumberPayer",
                table: "NewSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatorAccountPhoneNumber",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationNumberOwner",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationNumberPayer",
                table: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OperatorAccountPhoneNumber",
                table: "NewSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationNumberOwner",
                table: "NewSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationNumberPayer",
                table: "NewSubscriptionOrder");

            migrationBuilder.RenameColumn(
                name: "SimCardAction",
                table: "TransferToBusinessSubscriptionOrder",
                newName: "SIMCardAction");
        }
    }
}
