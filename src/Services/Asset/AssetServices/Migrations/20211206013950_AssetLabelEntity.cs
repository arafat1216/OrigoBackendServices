using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class AssetLabelEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetLabel",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    LabelId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetLabelId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLabel", x => new { x.AssetId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_AssetLabel_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetLabel_CustomerLabel_LabelId",
                        column: x => x.LabelId,
                        principalTable: "CustomerLabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AssetLabel_LabelId",
                table: "AssetLabel",
                column: "LabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetLabel");

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("1d4988aa-ca58-4f02-b2f5-8eba7d060b33"), new DateTime(2021, 12, 2, 10, 49, 30, 457, DateTimeKind.Local).AddTicks(824), new DateTime(2021, 12, 2, 10, 49, 30, 460, DateTimeKind.Local).AddTicks(4748) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("152a03f7-7bb0-4b97-b119-509939b87645"), new DateTime(2021, 12, 2, 10, 49, 30, 461, DateTimeKind.Local).AddTicks(5797), new DateTime(2021, 12, 2, 10, 49, 30, 461, DateTimeKind.Local).AddTicks(5818) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("2eaf71c1-6888-4883-8c13-0524bf6205c7"), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(550), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(566) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("77be706e-17a4-4634-b9dd-5c8d239825bf"), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1829), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1841) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("9e6ad69f-675c-435a-b5e7-ff94e5cefd85"), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1852), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1854) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("d046ecb6-39c6-488a-b611-4a4a16acd2af"), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1869), new DateTime(2021, 12, 2, 10, 49, 30, 462, DateTimeKind.Local).AddTicks(1871) });
        }
    }
}
