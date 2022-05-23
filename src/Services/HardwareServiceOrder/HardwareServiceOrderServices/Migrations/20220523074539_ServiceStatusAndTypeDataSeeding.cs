using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class ServiceStatusAndTypeDataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceStatuses",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 16, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001"), null, false, new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ServiceStatuses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
