using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class remove_space_from_featurepermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 2, "BasicAssetManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 2, "BasicAssetManagement ", 1, new Guid("00000000-0000-0000-0000-000000000001") });
        }
    }
}
