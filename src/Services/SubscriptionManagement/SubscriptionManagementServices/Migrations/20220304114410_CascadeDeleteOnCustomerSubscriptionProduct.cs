using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class CascadeDeleteOnCustomerSubscriptionProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_Operator_OperatorId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_Operator_OperatorId",
                table: "CustomerSubscriptionProduct",
                column: "OperatorId",
                principalTable: "Operator",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSubscriptionProduct_Operator_OperatorId",
                table: "CustomerSubscriptionProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSubscriptionProduct_Operator_OperatorId",
                table: "CustomerSubscriptionProduct",
                column: "OperatorId",
                principalTable: "Operator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
