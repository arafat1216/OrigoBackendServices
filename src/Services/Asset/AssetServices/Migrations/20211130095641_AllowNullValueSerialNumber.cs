using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class AllowNullValueSerialNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SerialKey",
                table: "SoftwareAsset",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "HardwareAsset",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AssetCategory",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "ParentAssetCategoryId", "UpdatedBy" },
                values: new object[] { 1, new Guid("409b702b-9fe5-4f0b-b6f6-3ef61b84c222"), new DateTime(2021, 11, 30, 10, 56, 40, 841, DateTimeKind.Local).AddTicks(5778), new Guid("00000000-0000-0000-0000-000000000000"), false, new DateTime(2021, 11, 30, 10, 56, 40, 844, DateTimeKind.Local).AddTicks(3894), null, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "AssetCategory",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "LastUpdatedDate", "ParentAssetCategoryId", "UpdatedBy" },
                values: new object[] { 2, new Guid("4af46115-aa3b-4d9e-b55d-f7da983984d3"), new DateTime(2021, 11, 30, 10, 56, 40, 845, DateTimeKind.Local).AddTicks(5617), new Guid("00000000-0000-0000-0000-000000000000"), false, new DateTime(2021, 11, 30, 10, 56, 40, 845, DateTimeKind.Local).AddTicks(5639), null, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "AssetCategoryTranslation",
                columns: new[] { "Id", "AssetCategoryId", "CreatedBy", "CreatedDate", "DeletedBy", "Description", "IsDeleted", "Language", "LastUpdatedDate", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, new Guid("e78434e2-d0bc-4cf1-9817-fc93d77f47d2"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(831), new Guid("00000000-0000-0000-0000-000000000000"), "Mobile phone", false, "EN", new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(847), "Mobile phone", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, 1, new Guid("556eed00-70ce-4631-900e-7333f361f099"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2416), new Guid("00000000-0000-0000-0000-000000000000"), "Mobiltelefon", false, "NO", new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2432), "Mobiltelefon", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, 2, new Guid("b9bb4b96-e659-4bfc-853f-c5f0b9196f42"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2446), new Guid("00000000-0000-0000-0000-000000000000"), "Tablet", false, "EN", new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2449), "Tablet", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, 2, new Guid("3cd74817-42d7-46de-a3c2-ead12c19f1c6"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2455), new Guid("00000000-0000-0000-0000-000000000000"), "Nettbrett", false, "NO", new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2457), "Nettbrett", new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "SerialKey",
                table: "SoftwareAsset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "HardwareAsset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
