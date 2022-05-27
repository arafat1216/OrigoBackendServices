using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class bookValue_Option_in_transactional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 5, "BasicBookValueManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "PartnerId", "ProductTypeId", "UpdatedBy" },
                values: new object[] { 4, new Guid("00000000-0000-0000-0000-000000000000"), 3, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 5, "en", "Allows organizations to Book value and Purchase price related tasks.", "Basic Book Value Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, "nb", "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", "Grunnleggende administrering av ikke-personlige assets", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 5, 3, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, 4, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "ProductRequiresOne",
                columns: new[] { "ProductId", "RequiresProductId", "UpdatedBy" },
                values: new object[] { 4, 2, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { "en", 4, "Allow Bookvalue and Purchas Price", "Book Value and Purchase Price", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "nb", 4, "Ett partner spesifikk abonnement-håndtering produkt", "Abonnement-håndtering", new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 4 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
