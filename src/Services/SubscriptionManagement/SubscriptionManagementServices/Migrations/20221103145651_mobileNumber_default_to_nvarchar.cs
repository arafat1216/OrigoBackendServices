using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class mobileNumber_default_to_nvarchar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "TransferToPrivateSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "ChangeSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "CancelSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "ActivateSimOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldComment: "A phone-number using E.164 format.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "TransferToPrivateSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "TransferToBusinessSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "ChangeSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "CancelSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "ActivateSimOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
