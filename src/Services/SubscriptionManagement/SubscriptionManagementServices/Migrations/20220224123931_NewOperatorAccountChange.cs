using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class NewOperatorAccountChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OperatorAccountId",
                table: "SubscriptionOrder",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OperatorAccountId",
                table: "PrivateToBusinessSubscriptionOrder",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OperatorAccountOwner",
                table: "PrivateToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorAccountPayer",
                table: "PrivateToBusinessSubscriptionOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatorAccountOwner",
                table: "PrivateToBusinessSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OperatorAccountPayer",
                table: "PrivateToBusinessSubscriptionOrder");

            migrationBuilder.AlterColumn<int>(
                name: "OperatorAccountId",
                table: "SubscriptionOrder",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OperatorAccountId",
                table: "PrivateToBusinessSubscriptionOrder",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
