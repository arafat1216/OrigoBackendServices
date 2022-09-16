using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class UpdatedConmodoServiceTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceProviderServiceType",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "ServiceProviderId", "ServiceTypeId", "UpdatedBy" },
                values: new object[] { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, 1, 2, new Guid("00000000-0000-0000-0000-000000000001") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceProviderServiceType",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
