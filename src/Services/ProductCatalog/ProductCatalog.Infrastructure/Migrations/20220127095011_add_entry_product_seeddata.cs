using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class add_entry_product_seeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 1 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "A partner product based subscription management", "Subscription management" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 1 },
                column: "Name",
                value: "Abonnement-håndtering");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "PartnerId", "ProductTypeId", "UpdatedBy" },
                values: new object[] { 2, new Guid("00000000-0000-0000-0000-000000000000"), 2, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 2, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, 2, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "ProductRequiresOne",
                columns: new[] { "ProductId", "RequiresProductId", "UpdatedBy" },
                values: new object[] { 1, 2, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductTranslation",
                columns: new[] { "Language", "ProductId", "Description", "Name", "UpdatedBy" },
                values: new object[,]
                {
                    { "en", 2, "Simple Asset Management for units purchased transactionally in Techstep's own WebShop.", "Entry", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "nb", 2, "Enkel Asset Management for enheter kjøpt transaksjonelt i Techstep egen nettbutikk.", "Entry", new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 2 });

            migrationBuilder.DeleteData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 2 });

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 1 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "A partner product based subscription managment", "Product subscription managment" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 1 },
                column: "Name",
                value: "Produkt abonnement-håndtering");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000001"));
        }
    }
}
