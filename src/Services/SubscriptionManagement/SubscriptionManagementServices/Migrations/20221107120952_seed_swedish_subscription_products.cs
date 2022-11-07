using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class seed_swedish_subscription_products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 4,
                column: "OperatorName",
                value: "Tele2 - SE");

            migrationBuilder.InsertData(
                table: "SubscriptionProduct",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "OperatorId", "SubscriptionName", "UpdatedBy" },
                values: new object[,]
                {
                    { 22, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Mobilt (fast pris)", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 23, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Mobilt (rörlig pris)", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 24, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Bredband", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 25, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Datakort", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 26, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Fastpris", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 27, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Sveriges Radio (Mobilabonnemang)", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 28, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Sveriges Radio (Mobilt Bredband)", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 29, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 4, "Sveriges Radio (Telematik)", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 30, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 2, "Jobbmobil", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "DataPackage",
                columns: new[] { "Id", "CreatedBy", "DataPackageName", "DeletedBy", "IsDeleted", "SubscriptionProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 64, new Guid("00000000-0000-0000-0000-000000000000"), "0,5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 65, new Guid("00000000-0000-0000-0000-000000000000"), "3 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 66, new Guid("00000000-0000-0000-0000-000000000000"), "10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 67, new Guid("00000000-0000-0000-0000-000000000000"), "40 GB - standard", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 68, new Guid("00000000-0000-0000-0000-000000000000"), "Obegränsad GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 69, new Guid("00000000-0000-0000-0000-000000000000"), "0 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 22, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 70, new Guid("00000000-0000-0000-0000-000000000000"), "5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 23, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 71, new Guid("00000000-0000-0000-0000-000000000000"), "0 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 23, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 72, new Guid("00000000-0000-0000-0000-000000000000"), "50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 24, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 73, new Guid("00000000-0000-0000-0000-000000000000"), "500 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 24, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 74, new Guid("00000000-0000-0000-0000-000000000000"), "1 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 25, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 75, new Guid("00000000-0000-0000-0000-000000000000"), "3 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 25, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 76, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 25, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 77, new Guid("00000000-0000-0000-0000-000000000000"), "5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 26, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 78, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 26, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 79, new Guid("00000000-0000-0000-0000-000000000000"), "50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 26, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 80, new Guid("00000000-0000-0000-0000-000000000000"), "100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 26, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 81, new Guid("00000000-0000-0000-0000-000000000000"), "3 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 30, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 82, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 30, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 83, new Guid("00000000-0000-0000-0000-000000000000"), "40 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 30, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 84, new Guid("00000000-0000-0000-0000-000000000000"), "Obegränsad GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 30, new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "Operator",
                keyColumn: "Id",
                keyValue: 4,
                column: "OperatorName",
                value: "Telenor - SE");
        }
    }
}
