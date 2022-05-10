using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class lifeCycleSettingChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryLifeCycleSettings");

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "LifeCycleSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinBuyoutPrice",
                table: "LifeCycleSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "LifeCycleSettings");

            migrationBuilder.DropColumn(
                name: "MinBuyoutPrice",
                table: "LifeCycleSettings");

            migrationBuilder.CreateTable(
                name: "CategoryLifeCycleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCategoryId = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LifeCycleSettingId = table.Column<int>(type: "int", nullable: true),
                    MinBuyoutPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryLifeCycleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryLifeCycleSettings_LifeCycleSettings_LifeCycleSettingId",
                        column: x => x.LifeCycleSettingId,
                        principalTable: "LifeCycleSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLifeCycleSettings_LifeCycleSettingId",
                table: "CategoryLifeCycleSettings",
                column: "LifeCycleSettingId");
        }
    }
}
