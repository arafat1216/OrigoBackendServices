using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class customer_standard_business_subscription_product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerStandardBusinessSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPackage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerStandardBusinessSubscriptionProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct",
                columns: table => new
                {
                    AddOnProductsId = table.Column<int>(type: "int", nullable: false),
                    CustomerStandardBusinessSubscriptionProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct", x => new { x.AddOnProductsId, x.CustomerStandardBusinessSubscriptionProductId });
                    table.ForeignKey(
                        name: "FK_CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct_CustomerStandardBusinessSubscriptionProduct_CustomerStan~",
                        column: x => x.CustomerStandardBusinessSubscriptionProductId,
                        principalTable: "CustomerStandardBusinessSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct_SubscriptionAddOnProduct_AddOnProductsId",
                        column: x => x.AddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings",
                column: "StandardBusinessSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct_CustomerStandardBusinessSubscriptionProductId",
                table: "CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct",
                column: "CustomerStandardBusinessSubscriptionProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOperatorSettings_CustomerStandardBusinessSubscriptionProduct_StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings",
                column: "StandardBusinessSubscriptionProductId",
                principalTable: "CustomerStandardBusinessSubscriptionProduct",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOperatorSettings_CustomerStandardBusinessSubscriptionProduct_StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings");

            migrationBuilder.DropTable(
                name: "CustomerStandardBusinessSubscriptionProductSubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "CustomerStandardBusinessSubscriptionProduct");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorSettings_StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings");

            migrationBuilder.DropColumn(
                name: "StandardBusinessSubscriptionProductId",
                table: "CustomerOperatorSettings");
        }
    }
}
