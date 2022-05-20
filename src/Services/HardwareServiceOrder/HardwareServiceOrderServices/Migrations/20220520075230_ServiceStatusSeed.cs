using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class ServiceStatusSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceStatuses",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 16, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), null, false, new Guid("00000000-0000-0000-0000-000000000000") }
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
