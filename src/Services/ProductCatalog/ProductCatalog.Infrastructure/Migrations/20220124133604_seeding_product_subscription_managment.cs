using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class seeding_product_subscription_managment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "PartnerId", "ProductTypeId", "UpdatedBy" },
                values: new object[] { 1, new Guid("00000000-0000-0000-0000-000000000001"), 3, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[] { 3, 1, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[] { "en", 1, "A partner product based subscription managment", "Product subscription managment", new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[] { "nb", 1, "Ett partner spesifikk abonnement-håndtering produkt", "Produkt abonnement-håndtering", new Guid("00000000-0000-0000-0000-000000000001") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 1 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 1 });

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
