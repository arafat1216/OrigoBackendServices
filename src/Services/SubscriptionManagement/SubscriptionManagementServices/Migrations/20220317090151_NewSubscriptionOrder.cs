using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class NewSubscriptionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimCardAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimCardAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewSubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPackageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountPayer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountOrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrivateSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    BusinessSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    SimCardNumber = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    SimCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimCardAddressId = table.Column<int>(type: "int", nullable: true),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalesforceTicketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewSubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewSubscriptionOrder_BusinessSubscription_BusinessSubscriptionId",
                        column: x => x.BusinessSubscriptionId,
                        principalTable: "BusinessSubscription",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NewSubscriptionOrder_PrivateSubscription_PrivateSubscriptionId",
                        column: x => x.PrivateSubscriptionId,
                        principalTable: "PrivateSubscription",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NewSubscriptionOrder_SimCardAddress_SimCardAddressId",
                        column: x => x.SimCardAddressId,
                        principalTable: "SimCardAddress",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NewSubscriptionOrderAddOnProducts",
                columns: table => new
                {
                    NewSubscriptionOrdersId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewSubscriptionOrderAddOnProducts", x => new { x.NewSubscriptionOrdersId, x.SubscriptionAddOnProductsId });
                    table.ForeignKey(
                        name: "FK_NewSubscriptionOrderAddOnProducts_NewSubscriptionOrder_NewSubscriptionOrdersId",
                        column: x => x.NewSubscriptionOrdersId,
                        principalTable: "NewSubscriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewSubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewSubscriptionOrder_BusinessSubscriptionId",
                table: "NewSubscriptionOrder",
                column: "BusinessSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_NewSubscriptionOrder_PrivateSubscriptionId",
                table: "NewSubscriptionOrder",
                column: "PrivateSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_NewSubscriptionOrder_SimCardAddressId",
                table: "NewSubscriptionOrder",
                column: "SimCardAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_NewSubscriptionOrderAddOnProducts_SubscriptionAddOnProductsId",
                table: "NewSubscriptionOrderAddOnProducts",
                column: "SubscriptionAddOnProductsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewSubscriptionOrderAddOnProducts");

            migrationBuilder.DropTable(
                name: "NewSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "SimCardAddress");
        }
    }
}
