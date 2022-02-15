using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class UniqueConstraintCustomerOperatorAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_AccountNumber",
                table: "CustomerOperatorAccount",
                columns: new[] { "OrganizationId", "AccountNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorAccount_OrganizationId_AccountNumber",
                table: "CustomerOperatorAccount");
        }
    }
}
