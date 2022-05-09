using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class adds_transactional_device_product_to_seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 4, "BasicNonPersonalAssetManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "PartnerId", "ProductTypeId", "UpdatedBy" },
                values: new object[] { 3, new Guid("00000000-0000-0000-0000-000000000000"), 2, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 4, "en", "Allows organizations to perform non-personal asset management tasks.", "Basic Non-personal Asset Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, "nb", "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", "Grunnleggende administrering av ikke-personlige assets", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, 3, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { "en", 3, "Lifecycle management for transasctional devices.", "Transactional Device Lifecycle Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "nb", 3, "Livssyklusadministrasjon for transaksjonelle enheter", "Transactional Device Lifecycle Management", new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 4, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 4, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 3 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 3 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
