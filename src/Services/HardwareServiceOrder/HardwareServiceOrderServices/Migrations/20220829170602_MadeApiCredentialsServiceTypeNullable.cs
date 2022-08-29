using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class MadeApiCredentialsServiceTypeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiCredentials_ServiceTypes_ServiceTypeId",
                table: "ApiCredentials");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ApiCredentials_CustomerServiceProviderId_ServiceTypeId",
                table: "ApiCredentials");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "ApiCredentials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ApiCredentials_CustomerServiceProviderId_ServiceTypeId",
                table: "ApiCredentials",
                columns: new[] { "CustomerServiceProviderId", "ServiceTypeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiCredentials_ServiceTypes_ServiceTypeId",
                table: "ApiCredentials",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiCredentials_ServiceTypes_ServiceTypeId",
                table: "ApiCredentials");

            migrationBuilder.DropIndex(
                name: "IX_ApiCredentials_CustomerServiceProviderId_ServiceTypeId",
                table: "ApiCredentials");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "ApiCredentials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ApiCredentials_CustomerServiceProviderId_ServiceTypeId",
                table: "ApiCredentials",
                columns: new[] { "CustomerServiceProviderId", "ServiceTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApiCredentials_ServiceTypes_ServiceTypeId",
                table: "ApiCredentials",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
