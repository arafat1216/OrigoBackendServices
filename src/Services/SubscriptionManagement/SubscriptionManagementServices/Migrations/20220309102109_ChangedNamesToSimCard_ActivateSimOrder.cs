using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class ChangedNamesToSimCard_ActivateSimOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SimType",
                table: "ActivateSimOrder",
                newName: "SimCardType");

            migrationBuilder.RenameColumn(
                name: "SimNumber",
                table: "ActivateSimOrder",
                newName: "SimCardNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SimCardType",
                table: "ActivateSimOrder",
                newName: "SimType");

            migrationBuilder.RenameColumn(
                name: "SimCardNumber",
                table: "ActivateSimOrder",
                newName: "SimNumber");
        }
    }
}
