using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class transfer_date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderExecutionDate",
                table: "SubscriptionOrder",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SIMCardNumber",
                table: "SubscriptionOrder",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TransferSubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NewOperatorAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferSubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferSubscriptionOrder_CustomerOperatorAccount_NewOperatorAccountId",
                        column: x => x.NewOperatorAccountId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferSubscriptionOrder_SubscriptionOrder_Id",
                        column: x => x.Id,
                        principalTable: "SubscriptionOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferSubscriptionOrder_NewOperatorAccountId",
                table: "TransferSubscriptionOrder",
                column: "NewOperatorAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferSubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrderExecutionDate",
                table: "SubscriptionOrder");

            migrationBuilder.DropColumn(
                name: "SIMCardNumber",
                table: "SubscriptionOrder");
        }
    }
}
