using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class SubscriptionProductChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionAddOnProduct_SubscriptionOrder_SubscriptionOrderId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "CustomerOperatorSettingsJoin");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionAddOnProduct_SubscriptionOrderId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.DropColumn(
                name: "SubscriptionOrderId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.AddColumn<int>(
                name: "BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GlobalSubscriptionProductId = table.Column<int>(type: "int", nullable: true),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSubscriptionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_SubscriptionProduct_GlobalSubscriptionProductId",
                        column: x => x.GlobalSubscriptionProductId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_SubscriptionProduct_Id",
                        column: x => x.Id,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAddOnProductSubscriptionOrder",
                columns: table => new
                {
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAddOnProductSubscriptionOrder", x => new { x.SubscriptionAddOnProductsId, x.SubscriptionOrdersId });
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrder_SubscriptionOrdersId",
                        column: x => x.SubscriptionOrdersId,
                        principalTable: "SubscriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOnProduct_BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct",
                column: "BelongsToSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptionProduct_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptionProduct_GlobalSubscriptionProductId",
                table: "CustomerSubscriptionProduct",
                column: "GlobalSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                column: "SubscriptionOrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionAddOnProduct_SubscriptionProduct_BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct",
                column: "BelongsToSubscriptionProductId",
                principalTable: "SubscriptionProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionAddOnProduct_SubscriptionProduct_BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "CustomerSubscriptionProduct");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionAddOnProduct_BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.DropColumn(
                name: "BelongsToSubscriptionProductId",
                table: "SubscriptionAddOnProduct");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionOrderId",
                table: "SubscriptionAddOnProduct",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerOperatorSettingsJoin",
                columns: table => new
                {
                    AvailableSubscriptionProductsId = table.Column<int>(type: "int", nullable: false),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOperatorSettingsJoin", x => new { x.AvailableSubscriptionProductsId, x.CustomerOperatorSettingsId });
                    table.ForeignKey(
                        name: "FK_CustomerOperatorSettingsJoin_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOperatorSettingsJoin_SubscriptionProduct_AvailableSubscriptionProductsId",
                        column: x => x.AvailableSubscriptionProductsId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOnProduct_SubscriptionOrderId",
                table: "SubscriptionAddOnProduct",
                column: "SubscriptionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettingsJoin_CustomerOperatorSettingsId",
                table: "CustomerOperatorSettingsJoin",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionAddOnProduct_SubscriptionOrder_SubscriptionOrderId",
                table: "SubscriptionAddOnProduct",
                column: "SubscriptionOrderId",
                principalTable: "SubscriptionOrder",
                principalColumn: "Id");
        }
    }
}
