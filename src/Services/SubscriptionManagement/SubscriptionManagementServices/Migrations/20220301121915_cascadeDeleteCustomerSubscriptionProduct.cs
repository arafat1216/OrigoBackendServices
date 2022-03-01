using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class cascadeDeleteCustomerSubscriptionProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id");
        }
    }
}
