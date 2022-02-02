using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class dataseeding_operator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Operator",
                columns: new[] { "Id", "Country", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "OperatorName", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 1, 13, 15, 30, 863, DateTimeKind.Local).AddTicks(7540), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 1, 13, 15, 30, 863, DateTimeKind.Local).AddTicks(7587), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - SE", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 1, 13, 15, 30, 863, DateTimeKind.Local).AddTicks(7591), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 1, 13, 15, 30, 863, DateTimeKind.Local).AddTicks(7594), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - SE", new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
