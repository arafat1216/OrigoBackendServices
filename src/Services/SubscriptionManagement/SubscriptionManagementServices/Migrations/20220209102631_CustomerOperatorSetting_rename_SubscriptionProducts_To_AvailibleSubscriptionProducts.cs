using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class CustomerOperatorSetting_rename_SubscriptionProducts_To_AvailibleSubscriptionProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersOperatorSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersOperatorSubscriptionProduct_SubscriptionProduct_SubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersOperatorSubscriptionProduct",
                table: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.DropIndex(
                name: "IX_CustomersOperatorSubscriptionProduct_SubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.RenameTable(
                name: "CustomersOperatorSubscriptionProduct",
                newName: "CustomerOperatorSettingsJoin");

            migrationBuilder.RenameColumn(
                name: "SubscriptionProductsId",
                table: "CustomerOperatorSettingsJoin",
                newName: "AvailableSubscriptionProductsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerOperatorSettingsJoin",
                table: "CustomerOperatorSettingsJoin",
                columns: new[] { "AvailableSubscriptionProductsId", "CustomerOperatorSettingsId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettingsJoin_CustomerOperatorSettingsId",
                table: "CustomerOperatorSettingsJoin",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOperatorSettingsJoin_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerOperatorSettingsJoin",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOperatorSettingsJoin_SubscriptionProduct_AvailableSubscriptionProductsId",
                table: "CustomerOperatorSettingsJoin",
                column: "AvailableSubscriptionProductsId",
                principalTable: "SubscriptionProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOperatorSettingsJoin_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomerOperatorSettingsJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOperatorSettingsJoin_SubscriptionProduct_AvailableSubscriptionProductsId",
                table: "CustomerOperatorSettingsJoin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerOperatorSettingsJoin",
                table: "CustomerOperatorSettingsJoin");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorSettingsJoin_CustomerOperatorSettingsId",
                table: "CustomerOperatorSettingsJoin");

            migrationBuilder.RenameTable(
                name: "CustomerOperatorSettingsJoin",
                newName: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.RenameColumn(
                name: "AvailableSubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct",
                newName: "SubscriptionProductsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersOperatorSubscriptionProduct",
                table: "CustomersOperatorSubscriptionProduct",
                columns: new[] { "CustomerOperatorSettingsId", "SubscriptionProductsId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomersOperatorSubscriptionProduct_SubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct",
                column: "SubscriptionProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersOperatorSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                table: "CustomersOperatorSubscriptionProduct",
                column: "CustomerOperatorSettingsId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersOperatorSubscriptionProduct_SubscriptionProduct_SubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct",
                column: "SubscriptionProductsId",
                principalTable: "SubscriptionProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
