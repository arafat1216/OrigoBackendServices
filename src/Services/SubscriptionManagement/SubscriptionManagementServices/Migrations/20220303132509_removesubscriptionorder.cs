using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class removesubscriptionorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "SubscriptionOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerSubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    DataPackageId = table.Column<int>(type: "int", nullable: false),
                    OperatorAccountId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SIMCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                        column: x => x.OperatorAccountId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                        column: x => x.CustomerSubscriptionProductId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                        column: x => x.DataPackageId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                column: "SubscriptionOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_CustomerSubscriptionProductId",
                table: "SubscriptionOrder",
                column: "CustomerSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_DataPackageId",
                table: "SubscriptionOrder",
                column: "DataPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId");
        }
    }
}
