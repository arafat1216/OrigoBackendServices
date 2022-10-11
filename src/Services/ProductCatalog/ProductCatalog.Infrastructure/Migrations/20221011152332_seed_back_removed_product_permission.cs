using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class seed_back_removed_product_permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[,]
                {
                    { 3, "BasicSubscriptionManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, "BasicNonPersonalAssetManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, "BasicBookValueManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, "BasicTransactionalAssetReturn", 2, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductExcludes",
                columns: new[] { "ExcludesProductId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 3, 2, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, 3, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, 3, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductRequiresOne",
                columns: new[] { "ProductId", "RequiresProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 2, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 1, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, 2, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, 3, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 3, "en", "Allows organizations to perform the basic subscription management tasks.", "Basic Subscription Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, "nb", "Lar en organisasjon utføre grunnleggende behandling av abonnementer.", "Grunnleggende abonnement-håndtering", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, "en", "Allows organizations to perform non-personal asset management tasks.", "Basic Non-personal Asset Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, "nb", "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", "Grunnleggende administrering av ikke-personlige assets", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, "en", "Allows organizations to Book value and Purchase price related tasks.", "Basic Book Value Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, "nb", "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", "Håndtering av Bokført verdi", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, "en", "Makes it possible to use the 'return' functionality on supported transactional devices. This enables the most basic 'internal return' functionality, and is required when using/activating some of the more advanced return functionality.", "Basic asset return (transactional)", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, "nb", "Gjør det mulig å benytte 'returner' funksjonaliteten på støttede transaksjonelle enheter. Dette tilgjengeliggjør den mest grunnleggende 'intern retur' funksjonaliteten, og er nødvendig for å kunne bruke/aktivere noen av de mer avanserte retur funksjonalitetene.", "Grunnleggende asset retur (transaksjonell)", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 3, 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, 4, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, 5, new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 3, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 3, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 4, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 4, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 6, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 6, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductExcludes",
                keyColumns: new[] { "ExcludesProductId", "ProductId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ProductExcludes",
                keyColumns: new[] { "ExcludesProductId", "ProductId" },
                keyValues: new object[] { 2, 3 });

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
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
