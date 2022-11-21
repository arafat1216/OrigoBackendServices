using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class add_webshop_access_feature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 17, "WebshopAccess", 1, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[] { 17, "en", "Allows Users to visit web shop page.", "Webshop Access", new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[] { 17, "nb", "Lar brukere besøke nettbutikksiden.", "Tilgang til nettbutikk", new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[] { 17, 3, new Guid("00000000-0000-0000-0000-000000000001") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 17, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 17, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 17, 3 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
