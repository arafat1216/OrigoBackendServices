using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class max_string_lengths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SimCardCountry",
                table: "TransferToBusinessSubscriptionOrder",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: true,
                comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "TransferToBusinessSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: true,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "PrivateSubscription",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "PrivateSubscription",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "OrderSimSubscriptionOrder",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardReceiverCountry",
                table: "NewSubscriptionOrder",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: true,
                comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "NewSubscriptionOrder",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: true,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "BusinessSubscription",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardNumber",
                table: "ActivateSimOrder",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SimCardCountry",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldNullable: true,
                oldComment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

            migrationBuilder.AlterColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "TransferToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldNullable: true,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "PrivateSubscription",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldMaxLength: 320,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "PrivateSubscription",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "OrderSimSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardReceiverCountry",
                table: "NewSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldNullable: true,
                oldComment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

            migrationBuilder.AlterColumn<string>(
                name: "OperatorAccountPhoneNumber",
                table: "NewSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldNullable: true,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "BusinessSubscription",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardNumber",
                table: "ActivateSimOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(22)",
                oldMaxLength: 22);
        }
    }
}
