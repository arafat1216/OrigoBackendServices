using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class customer_settings_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CustomerSettings");

            migrationBuilder.InsertData(
                table: "ServiceProviders",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "OrganizationId", "UpdatedBy" },
                values: new object[] { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ServiceProviders",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "OrganizationId", "UpdatedBy" },
                values: new object[] { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerServiceProvider_ServiceProviderId",
                table: "CustomerServiceProvider",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerServiceProvider_ServiceProviders_ServiceProviderId",
                table: "CustomerServiceProvider",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerServiceProvider_ServiceProviders_ServiceProviderId",
                table: "CustomerServiceProvider");

            migrationBuilder.DropIndex(
                name: "IX_CustomerServiceProvider_ServiceProviderId",
                table: "CustomerServiceProvider");

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "CustomerSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
