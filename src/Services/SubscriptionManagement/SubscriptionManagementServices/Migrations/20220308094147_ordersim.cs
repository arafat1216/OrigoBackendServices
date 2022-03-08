using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class ordersim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferToPrivateSubscriptionOrder_PrivateSubscription_UserInfoId",
                table: "TransferToPrivateSubscriptionOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferToPrivateSubscriptionOrder",
                table: "TransferToPrivateSubscriptionOrder");

            migrationBuilder.RenameTable(
                name: "TransferToPrivateSubscriptionOrder",
                newName: "OrderSimSubscriptionOrder");

            migrationBuilder.RenameIndex(
                name: "IX_TransferToPrivateSubscriptionOrder_UserInfoId",
                table: "OrderSimSubscriptionOrder",
                newName: "IX_OrderSimSubscriptionOrder_UserInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderSimSubscriptionOrder",
                table: "OrderSimSubscriptionOrder",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrderSimSubscriptionOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendToName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesforceTicketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSimSubscriptionOrders", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSimSubscriptionOrder_PrivateSubscription_UserInfoId",
                table: "OrderSimSubscriptionOrder",
                column: "UserInfoId",
                principalTable: "PrivateSubscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderSimSubscriptionOrder_PrivateSubscription_UserInfoId",
                table: "OrderSimSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "OrderSimSubscriptionOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderSimSubscriptionOrder",
                table: "OrderSimSubscriptionOrder");

            migrationBuilder.RenameTable(
                name: "OrderSimSubscriptionOrder",
                newName: "TransferToPrivateSubscriptionOrder");

            migrationBuilder.RenameIndex(
                name: "IX_OrderSimSubscriptionOrder_UserInfoId",
                table: "TransferToPrivateSubscriptionOrder",
                newName: "IX_TransferToPrivateSubscriptionOrder_UserInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferToPrivateSubscriptionOrder",
                table: "TransferToPrivateSubscriptionOrder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferToPrivateSubscriptionOrder_PrivateSubscription_UserInfoId",
                table: "TransferToPrivateSubscriptionOrder",
                column: "UserInfoId",
                principalTable: "PrivateSubscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
