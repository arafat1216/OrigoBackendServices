using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class LastUpdatedDate_UTCDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("ccdebe93-56da-4cff-b0b5-186086e89775"), new DateTime(2021, 12, 20, 11, 21, 8, 929, DateTimeKind.Local).AddTicks(8364), new DateTime(2021, 12, 20, 11, 21, 8, 931, DateTimeKind.Local).AddTicks(6170) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("05910ac1-5401-47cc-b150-e0717862b761"), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(4595), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(4614) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("925f1883-26bb-4112-bb2c-fe18edcc84e7"), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(8380), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(8394) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("f41d380d-81ba-4a22-8a0d-840f5d2ac92d"), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9657), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9670) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("ccace718-34d6-4736-8313-13008a54f2ab"), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9682), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9683) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("98f544c2-7073-4127-b9a8-0ddef3319437"), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9688), new DateTime(2021, 12, 20, 11, 21, 8, 932, DateTimeKind.Local).AddTicks(9690) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

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
    }
}
