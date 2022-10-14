using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class added_sql_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_ExternalId",
                table: "Users",
                column: "ExternalId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LifeCycleSetting_ExternalId",
                table: "LifeCycleSetting",
                column: "ExternalId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CustomerSettings_CustomerId",
                table: "CustomerSettings",
                column: "CustomerId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Assets_ExternalId",
                table: "Assets",
                column: "ExternalId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AssetLifeCycles_ExternalId",
                table: "AssetLifeCycles",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLifeCycles_CustomerId",
                table: "AssetLifeCycles",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_ExternalId",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_LifeCycleSetting_ExternalId",
                table: "LifeCycleSetting");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CustomerSettings_CustomerId",
                table: "CustomerSettings");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Assets_ExternalId",
                table: "Assets");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AssetLifeCycles_ExternalId",
                table: "AssetLifeCycles");

            migrationBuilder.DropIndex(
                name: "IX_AssetLifeCycles_CustomerId",
                table: "AssetLifeCycles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");
        }
    }
}
