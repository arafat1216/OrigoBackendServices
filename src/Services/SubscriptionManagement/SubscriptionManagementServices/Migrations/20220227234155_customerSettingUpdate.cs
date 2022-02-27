using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class customerSettingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                table: "SubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_SubscriptionProductId",
                table: "SubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                table: "SubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                table: "SubscriptionOrderAddOnProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrderAddOnProducts_SubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionOrderAddOnProducts");

            migrationBuilder.DropTable(
                name: "CustomersOperatorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionOrder_SubscriptionProductId",
                table: "SubscriptionOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionOrderAddOnProducts",
                table: "SubscriptionOrderAddOnProducts");

            migrationBuilder.RenameTable(
                name: "SubscriptionOrderAddOnProducts",
                newName: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionOrderAddOnProducts_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                newName: "IX_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrdersId");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardNumber",
                table: "SubscriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "SubscriptionOrder",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SubscriptionOrder",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<int>(
                name: "CustomerSubscriptionProductId",
                table: "SubscriptionOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerOperatorSettingId",
                table: "CustomerOperatorAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionAddOnProductSubscriptionOrder",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                columns: new[] { "SubscriptionAddOnProductsId", "SubscriptionOrdersId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_CustomerSubscriptionProductId",
                table: "SubscriptionOrder",
                column: "CustomerSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorAccount_CustomerOperatorSettingId",
                table: "CustomerOperatorAccount",
                column: "CustomerOperatorSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOperatorAccount_CustomerOperatorSettings_CustomerOperatorSettingId",
                table: "CustomerOperatorAccount",
                column: "CustomerOperatorSettingId",
                principalTable: "CustomerOperatorSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                column: "SubscriptionAddOnProductsId",
                principalTable: "SubscriptionAddOnProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                column: "SubscriptionOrdersId",
                principalTable: "SubscriptionOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId",
                principalTable: "CustomerOperatorAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                table: "SubscriptionOrder",
                column: "CustomerSubscriptionProductId",
                principalTable: "CustomerSubscriptionProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                table: "SubscriptionOrder",
                column: "DataPackageId",
                principalTable: "DataPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOperatorAccount_CustomerOperatorSettings_CustomerOperatorSettingId",
                table: "CustomerOperatorAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                table: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                table: "SubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                table: "SubscriptionOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                table: "SubscriptionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionOrder_CustomerSubscriptionProductId",
                table: "SubscriptionOrder");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorAccount_CustomerOperatorSettingId",
                table: "CustomerOperatorAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionAddOnProductSubscriptionOrder",
                table: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "CustomerSubscriptionProductId",
                table: "SubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "CustomerOperatorSettingId",
                table: "CustomerOperatorAccount");

            migrationBuilder.RenameTable(
                name: "SubscriptionAddOnProductSubscriptionOrder",
                newName: "SubscriptionOrderAddOnProducts");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionOrderAddOnProducts",
                newName: "IX_SubscriptionOrderAddOnProducts_SubscriptionOrdersId");

            migrationBuilder.AlterColumn<string>(
                name: "SimCardNumber",
                table: "SubscriptionOrder",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "SubscriptionOrder",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SubscriptionOrder",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionOrderAddOnProducts",
                table: "SubscriptionOrderAddOnProducts",
                columns: new[] { "SubscriptionAddOnProductsId", "SubscriptionOrdersId" });

            migrationBuilder.CreateTable(
                name: "CustomersOperatorAccounts",
                columns: table => new
                {
                    CustomerOperatorAccountsId = table.Column<int>(type: "int", nullable: false),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersOperatorAccounts", x => new { x.CustomerOperatorAccountsId, x.CustomerOperatorSettingsId });
                    table.ForeignKey(
                        name: "FK_CustomersOperatorAccounts_CustomerOperatorAccount_CustomerOperatorAccountsId",
                        column: x => x.CustomerOperatorAccountsId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersOperatorAccounts_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_SubscriptionProductId",
                table: "SubscriptionOrder",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersOperatorAccounts_CustomerOperatorSettingsId",
                table: "CustomersOperatorAccounts",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId",
                principalTable: "CustomerOperatorAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_SubscriptionProductId",
                table: "SubscriptionOrder",
                column: "SubscriptionProductId",
                principalTable: "CustomerSubscriptionProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                table: "SubscriptionOrder",
                column: "DataPackageId",
                principalTable: "DataPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                table: "SubscriptionOrderAddOnProducts",
                column: "SubscriptionAddOnProductsId",
                principalTable: "SubscriptionAddOnProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionOrderAddOnProducts_SubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionOrderAddOnProducts",
                column: "SubscriptionOrdersId",
                principalTable: "SubscriptionOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
