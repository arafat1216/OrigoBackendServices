using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class RestructuredAndUpdatedSeedingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 3, "nb" },
                column: "Description",
                value: "Lar en organisasjon utføre grunnleggende behandling av abonnementer.");

            migrationBuilder.InsertData(
                table: "FeatureTypes",
                columns: new[] { "Id", "UpdatedBy" },
                values: new object[] { 2, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 8, "BasicHardwareRepair", 1, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 3 },
                column: "Description",
                value: "Lifecycle management for transactional devices.");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 4 },
                column: "Description",
                value: "Allow book value and Purchase Price.");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                column: "Description",
                value: "Tilgjengeliggjør bokført verdi og kjøpspris.");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 2 },
                column: "Description",
                value: "A base-module. These types of products are stand-alone product offerings, and is typically considered a 'base' or 'core' product.");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 2 },
                column: "Description",
                value: "En basis-modul. Dette er frittstående produkter, og er normalt ett basis- eller kjerne-produkt.");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 3 },
                column: "Description",
                value: "An option is a addon-product enables extra functionality in one or more of the modules.");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 3 },
                column: "Description",
                value: "En opsjon er ett tillegsprodukt som aktiverer ekstra funksjonalitet i en eller flere moduler.");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "PartnerId", "ProductTypeId", "UpdatedBy" },
                values: new object[] { 5, new Guid("5741b4a1-4eef-4fc2-b1b8-0ba7f41ed93c"), 3, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 8, "en", "Makes it possible for organizations to create ordinary repair/service-orders on supported hardware assets.", "Basic Hardware Repair", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 8, "nb", "Gjør det mulig for organisasjoner å opprette ordinære reparasjon/service-ordrer på støttede hardware assets.", "Grunnlegende Hardware Reparasjon", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "FeatureTypeTranslation",
                columns: new[] { "FeatureTypeId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 2, "en", "This type contains functionality that enables return, recycle and other similar functionality for assets.", "Asset return", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, "nb", "Denne typen inneholder funksjoner som muliggjør retur, resirkulering og andre lignende funksjonaliteter på assets.", "Asset retur", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[,]
                {
                    { 6, "BasicTransactionalAssetReturn", 2, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, "RecycleAndWipeAssetReturn", 2, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[] { 8, 3, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { "en", 5, "Allows 'Recycle & Wipe' to be selected as a aftermarket option in the organizations asset-category settings.", "Recycle & Wipe", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "nb", 5, "Tilgjengeliggjør 'Recycle & Wipe' som ett alternativ inne i organisasjonens asset-kategori innstillinger.", "Recycle & Wipe", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "FeatureRequiresOne",
                columns: new[] { "FeatureId", "RequiresFeatureId", "UpdatedBy" },
                values: new object[] { 7, 6, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 6, "en", "Makes it possible to use the 'return' functionality on supported transactional devices. This enables the most basic 'internal return' functionality, and is required when using/activating some of the more advanced return functionality.", "Basic asset return (transactional)", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, "nb", "Gjør det mulig å benytte 'returner' funksjonaliteten på støttede transaksjonelle enheter. Dette tilgjengeliggjør den mest grunnleggende 'intern retur' funksjonaliteten, og er nødvendig for å kunne bruke/aktivere noen av de mer avanserte retur funksjonalitetene.", "Grunnleggende asset retur (transaksjonell)", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, "en", "Makes it possible to return supported assets through 'Recycle & Wipe' service-orders.", "Recycle & Wipe asset return", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, "nb", "Gjør det mulig å returnere støttede enheter via 'Recycle & Wipe' service-ordrer.", "Recycle & Wipe asset retur", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 6, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, 5, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, 5, new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureRequiresOne",
                keyColumns: new[] { "FeatureId", "RequiresFeatureId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 6, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 6, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 7, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 7, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 8, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 8, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTypeTranslation",
                keyColumns: new[] { "FeatureTypeId", "Language" },
                keyValues: new object[] { 2, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTypeTranslation",
                keyColumns: new[] { "FeatureTypeId", "Language" },
                keyValues: new object[] { 2, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 5 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 5 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FeatureTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 3, "nb" },
                column: "Description",
                value: "Lar en organisasjon utføre grunnleggende behandling av abonnomenter.");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 3 },
                column: "Description",
                value: "Lifecycle management for transasctional devices.");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 4 },
                column: "Description",
                value: "Allow Bookvalue and Purchas Price");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                column: "Description",
                value: "Tilgjengeliggjør bokført verdi og kjøpspris");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 2 },
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 2 },
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 3 },
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 3 },
                column: "Description",
                value: null);
        }
    }
}
