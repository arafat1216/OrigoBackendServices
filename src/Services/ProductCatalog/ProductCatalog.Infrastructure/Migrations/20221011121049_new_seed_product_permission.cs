using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class new_seed_product_permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureRequiresOne",
                keyColumns: new[] { "FeatureId", "RequiresFeatureId" },
                keyValues: new object[] { 7, 6 });

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
                keyValues: new object[] { 4, 2 });

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

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[,]
                {
                    { 9, "EmployeeAccess", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, "DepartmentStructure", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, "OnAndOffboarding", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, "BuyoutAsset", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, "AssetManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, "SubscriptionManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, "AssetBookValue", 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 16, "InternalAssetReturn", 2, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { 9, "en", "Enables the various employee functionality/access.", "Employee Access", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 9, "nb", "Muliggjør de forskjellige funksjonalitetene og adgang for ansatte.", "Ansatt brukertilgang", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, "en", "Enables the various department functionality/access.", "Department Structure", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, "nb", "Muliggjør de forskjellige funksjonalitetene og adgang for avdelinger i selskapet.", "Avdelings struktur", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, "en", "Enables functionality for on- and offboarding of user and customer.", "Onboarding and Offboarding", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, "nb", "Muliggjør de forskjellige funksjonalitetene rund en bruker og/eller kundes oppstart eller avsluttning.", "Oppstart og avluttning", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, "en", "Makes it possible for users to buy out assets.", "Buyout Asset", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, "nb", "Muliggjør oppkjøp av assets for brukere.", "Utkjøp av asset", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, "en", "Allows organizations to perform asset management tasks.", "Asset Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, "nb", "Lar en organisasjon utføre behandling av assets.", "Asset-håndtering", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, "en", "Allows organizations to perform subscription management tasks.", "Subscription Management", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, "nb", "Gir kunder tilgang til administrering av abonnementer.", "Abonnent håndtering", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, "en", "Allows organizations to book value and purchase price related tasks.", "Asset Book Value", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, "nb", "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", "Håndtering av bokført verdi", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 16, "en", "The organization handles the return of assets internally.", "Internal Asset Return", new Guid("00000000-0000-0000-0000-000000000001") },
                    { 16, "nb", "Organisasjonen håndterer return av asset internt.", "Intern asset retur", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 9, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, 4, new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 9, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 9, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 10, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 10, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 11, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 11, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 12, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 12, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 13, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 13, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 14, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 14, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 15, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 15, "nb" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 16, "en" });

            migrationBuilder.DeleteData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 16, "nb" });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 11, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 15, 4 });

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: 16);

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
                    { 4, 2, new Guid("00000000-0000-0000-0000-000000000001") }
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
    }
}
