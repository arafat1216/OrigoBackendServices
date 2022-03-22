using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class madedeletedBynullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "CustomerLabel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetLabel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetCategoryTranslation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Asset",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("f7474a3b-2da6-444e-b2a1-590ca2c5eb00"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9005), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9050) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("24af9a79-a4d0-4d10-ace8-a249679fedaa"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9111), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9114) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("c12bfc55-1d63-41c0-bd2b-87a41c623112"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9147), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9151) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("d9c3892a-c056-4f73-bc26-7b15c45bae17"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9168), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9170) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("3b1e77ac-948b-450f-9b73-8752336054b1"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9176), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9178) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("13446b9e-4a92-450f-b508-55ff58b8b2b6"), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9183), new DateTime(2022, 3, 18, 14, 45, 46, 630, DateTimeKind.Local).AddTicks(9186) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "CustomerLabel",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetLabel",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetCategoryTranslation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "AssetCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Asset",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
    }
}
