using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class dataseeding_operator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "CustomerOperatorAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Operator",
                columns: new[] { "Id", "Country", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "OperatorName", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 2, 19, 43, 10, 527, DateTimeKind.Local).AddTicks(7887), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 2, 19, 43, 10, 527, DateTimeKind.Local).AddTicks(7930), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - SE", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 2, 19, 43, 10, 527, DateTimeKind.Local).AddTicks(7933), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 2, 19, 43, 10, 527, DateTimeKind.Local).AddTicks(7936), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - SE", new Guid("00000000-0000-0000-0000-000000000000") }
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

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "CustomerOperatorAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
