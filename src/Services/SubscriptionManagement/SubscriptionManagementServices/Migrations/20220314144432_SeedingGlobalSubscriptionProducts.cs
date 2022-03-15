using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class SeedingGlobalSubscriptionProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "CustomerOperatorAccount",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "SubscriptionProduct",
                columns: new[] { "Id", "CreatedBy", "DeletedBy", "IsDeleted", "OperatorId", "SubscriptionName", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift fri +", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift fri L", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total extra", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift small", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total A", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total B", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total C", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total D", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total E", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total+ A", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total+ B", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total+ C", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift total+ D", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift flyt M", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 16, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift flyt S", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 17, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 3, "Bedrift XS", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 18, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 1, "Telia smart", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 19, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 1, "Telia Smart Avtalepris L", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 20, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 1, "Mobilt bredbånd", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 21, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000"), false, 1, "Telia Ubegrenset", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "DataPackage",
                columns: new[] { "Id", "CreatedBy", "DataPackageName", "DeletedBy", "IsDeleted", "SubscriptionProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000000"), "1 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000000"), "4 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000000"), "8 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000000"), "20 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000000"), "40 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000000"), "60 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 1, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 1 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 2 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 3 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 25 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 35 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 16, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 75 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 17, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 18, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 150 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 19, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 200 MB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 20, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 1 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 21, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 3 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 22, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 23, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 24, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 25, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 25 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 26, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 35 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 27, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 28, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 75 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 29, new Guid("00000000-0000-0000-0000-000000000000"), "Surf 100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 4, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 30, new Guid("00000000-0000-0000-0000-000000000000"), "100 MB", new Guid("00000000-0000-0000-0000-000000000000"), false, 5, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 31, new Guid("00000000-0000-0000-0000-000000000000"), "200 MB", new Guid("00000000-0000-0000-0000-000000000000"), false, 5, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 32, new Guid("00000000-0000-0000-0000-000000000000"), "10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 6, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 33, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 6, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 34, new Guid("00000000-0000-0000-0000-000000000000"), "20 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 6, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 35, new Guid("00000000-0000-0000-0000-000000000000"), "25 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 7, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 36, new Guid("00000000-0000-0000-0000-000000000000"), "35 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 7, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 37, new Guid("00000000-0000-0000-0000-000000000000"), "40 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 7, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 38, new Guid("00000000-0000-0000-0000-000000000000"), "30 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 8, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 39, new Guid("00000000-0000-0000-0000-000000000000"), "50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 8, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 40, new Guid("00000000-0000-0000-0000-000000000000"), "100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 8, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 41, new Guid("00000000-0000-0000-0000-000000000000"), "60 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 9, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 42, new Guid("00000000-0000-0000-0000-000000000000"), "100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 10, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "DataPackage",
                columns: new[] { "Id", "CreatedBy", "DataPackageName", "DeletedBy", "IsDeleted", "SubscriptionProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 43, new Guid("00000000-0000-0000-0000-000000000000"), "5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 11, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 44, new Guid("00000000-0000-0000-0000-000000000000"), "10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 12, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 45, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 13, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 46, new Guid("00000000-0000-0000-0000-000000000000"), "100 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 14, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 47, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris S", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 48, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris small+", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 49, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris M", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 50, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris Basis", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 51, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris Fri", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 52, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris Respons", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 53, new Guid("00000000-0000-0000-0000-000000000000"), "Avtalepris Variabel", new Guid("00000000-0000-0000-0000-000000000000"), false, 18, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 54, new Guid("00000000-0000-0000-0000-000000000000"), "10 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 19, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 55, new Guid("00000000-0000-0000-0000-000000000000"), "15 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 19, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 56, new Guid("00000000-0000-0000-0000-000000000000"), "30 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 19, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 57, new Guid("00000000-0000-0000-0000-000000000000"), "50 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 19, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 58, new Guid("00000000-0000-0000-0000-000000000000"), "5 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 59, new Guid("00000000-0000-0000-0000-000000000000"), "20 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 60, new Guid("00000000-0000-0000-0000-000000000000"), "40 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 61, new Guid("00000000-0000-0000-0000-000000000000"), "80 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 62, new Guid("00000000-0000-0000-0000-000000000000"), "150 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 63, new Guid("00000000-0000-0000-0000-000000000000"), "500 GB", new Guid("00000000-0000-0000-0000-000000000000"), false, 20, new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "DataPackage",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SubscriptionProduct",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "CustomerOperatorAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);
        }
    }
}
