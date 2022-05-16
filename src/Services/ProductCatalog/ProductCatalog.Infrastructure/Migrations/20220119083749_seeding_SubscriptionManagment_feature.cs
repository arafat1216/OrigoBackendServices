using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class seeding_SubscriptionManagment_feature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "AccessControlPermissionNode", "FeatureTypeId", "UpdatedBy" },
                values: new object[] { 3, "BasicSubscriptionManagement", 1, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[] { 3, "en", "Allows organizations to perform the basic subscription management tasks.", "Basic Subscription Management", new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "FeatureTranslation",
                columns: new[] { "FeatureId", "Language", "Description", "Name", "UpdatedBy" },
                values: new object[] { 3, "nb", "Lar en organisasjon utføre grunnleggende behandling av abonnomenter.", "Grunnleggende abonnement-håndtering", new Guid("00000000-0000-0000-0000-000000000001") });
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
                table: "Features",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
