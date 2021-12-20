using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class AddedUTCTimeForLastUpdatedDate : Migration
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
                values: new object[] { new Guid("b93f0bf6-1580-4861-bc6d-fd2220fa80d6"), new DateTime(2021, 12, 17, 14, 55, 8, 15, DateTimeKind.Local).AddTicks(4723), new DateTime(2021, 12, 17, 14, 55, 8, 17, DateTimeKind.Local).AddTicks(998) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("d37db446-6005-4678-99d5-470c3d156372"), new DateTime(2021, 12, 17, 14, 55, 8, 17, DateTimeKind.Local).AddTicks(8471), new DateTime(2021, 12, 17, 14, 55, 8, 17, DateTimeKind.Local).AddTicks(8484) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("f36b8199-f67f-4e59-86e2-0e1e4189ae06"), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(1830), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(1842) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("c16af0c1-6d0d-44f4-88db-2a354e8da0b8"), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2746), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2756) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("55443008-687d-4c92-a41c-9c374109078b"), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2766), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2769) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("ebe65417-238c-4195-a266-e3269f8a71f5"), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2772), new DateTime(2021, 12, 17, 14, 55, 8, 18, DateTimeKind.Local).AddTicks(2774) });
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
