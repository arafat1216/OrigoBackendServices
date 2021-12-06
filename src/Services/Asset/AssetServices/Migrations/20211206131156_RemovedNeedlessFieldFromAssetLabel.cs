using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class RemovedNeedlessFieldFromAssetLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetLabelId",
                table: "AssetLabel");

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("f3793f65-1681-4fd3-997a-3f6c58e08eda"), new DateTime(2021, 12, 6, 14, 11, 55, 91, DateTimeKind.Local).AddTicks(2832), new DateTime(2021, 12, 6, 14, 11, 55, 95, DateTimeKind.Local).AddTicks(3144) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("3efb81c8-66a4-4536-94e2-4472a9c693cf"), new DateTime(2021, 12, 6, 14, 11, 55, 96, DateTimeKind.Local).AddTicks(8376), new DateTime(2021, 12, 6, 14, 11, 55, 96, DateTimeKind.Local).AddTicks(8420) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("6b6e9448-973a-4204-86e2-3bcb4ffbbf9f"), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(5148), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(5174) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("6870b4e7-b8e0-47f6-9fda-960054d6864c"), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8115), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8140) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("cff07875-3e86-4834-b066-9da308f3ef05"), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8166), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8169) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("fc9f3d2c-5480-43b6-9dbc-6a22ff50300e"), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8175), new DateTime(2021, 12, 6, 14, 11, 55, 97, DateTimeKind.Local).AddTicks(8179) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetLabelId",
                table: "AssetLabel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("985df217-0868-4359-86df-8a7b417db014"), new DateTime(2021, 12, 6, 2, 39, 49, 186, DateTimeKind.Local).AddTicks(7055), new DateTime(2021, 12, 6, 2, 39, 49, 192, DateTimeKind.Local).AddTicks(7030) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("c0ad55b5-c587-4dc6-9743-7833aa47d6b0"), new DateTime(2021, 12, 6, 2, 39, 49, 195, DateTimeKind.Local).AddTicks(4113), new DateTime(2021, 12, 6, 2, 39, 49, 195, DateTimeKind.Local).AddTicks(4201) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("384622b3-fdcd-4409-a727-304448f04743"), new DateTime(2021, 12, 6, 2, 39, 49, 196, DateTimeKind.Local).AddTicks(8959), new DateTime(2021, 12, 6, 2, 39, 49, 196, DateTimeKind.Local).AddTicks(9025) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("964f4cc6-285a-41df-9be4-f074832dfa33"), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2861), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2900) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("860b6b08-5e2c-4045-95b6-b72459c5f9a5"), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2942), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2950) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("aafaeaac-d123-42ee-8911-3073f8a7f762"), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2966), new DateTime(2021, 12, 6, 2, 39, 49, 197, DateTimeKind.Local).AddTicks(2973) });
        }
    }
}
