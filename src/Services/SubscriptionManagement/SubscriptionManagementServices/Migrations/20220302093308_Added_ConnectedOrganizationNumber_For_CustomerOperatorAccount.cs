using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class Added_ConnectedOrganizationNumber_For_CustomerOperatorAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectedOrganizationNumber",
                table: "CustomerOperatorAccount",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectedOrganizationNumber",
                table: "CustomerOperatorAccount");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
