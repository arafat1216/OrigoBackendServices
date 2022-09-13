using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class Idx_CustomerOperatorAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_AccountNumber",
                table: "CustomerOperatorAccount");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_OperatorId_AccountNumber",
                table: "CustomerOperatorAccount",
                columns: new[] { "OrganizationId", "OperatorId", "AccountNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_OperatorId_AccountNumber",
                table: "CustomerOperatorAccount");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_AccountNumber",
                table: "CustomerOperatorAccount",
                columns: new[] { "OrganizationId", "AccountNumber" },
                unique: true);
        }
    }
}
