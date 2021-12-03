using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class CustomerLabelEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerLabel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label_Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Label_Color = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLabel", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLabel");

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("409b702b-9fe5-4f0b-b6f6-3ef61b84c222"), new DateTime(2021, 11, 30, 10, 56, 40, 841, DateTimeKind.Local).AddTicks(5778), new DateTime(2021, 11, 30, 10, 56, 40, 844, DateTimeKind.Local).AddTicks(3894) });

            migrationBuilder.UpdateData(
                table: "AssetCategory",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("4af46115-aa3b-4d9e-b55d-f7da983984d3"), new DateTime(2021, 11, 30, 10, 56, 40, 845, DateTimeKind.Local).AddTicks(5617), new DateTime(2021, 11, 30, 10, 56, 40, 845, DateTimeKind.Local).AddTicks(5639) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("e78434e2-d0bc-4cf1-9817-fc93d77f47d2"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(831), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(847) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("556eed00-70ce-4631-900e-7333f361f099"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2416), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2432) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("b9bb4b96-e659-4bfc-853f-c5f0b9196f42"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2446), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2449) });

            migrationBuilder.UpdateData(
                table: "AssetCategoryTranslation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastUpdatedDate" },
                values: new object[] { new Guid("3cd74817-42d7-46de-a3c2-ead12c19f1c6"), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2455), new DateTime(2021, 11, 30, 10, 56, 40, 846, DateTimeKind.Local).AddTicks(2457) });
        }
    }
}
