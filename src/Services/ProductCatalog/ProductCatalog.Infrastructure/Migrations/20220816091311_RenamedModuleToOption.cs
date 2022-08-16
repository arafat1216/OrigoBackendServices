using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class RenamedModuleToOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 2 },
                column: "Name",
                value: "Product");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 2 },
                column: "Name",
                value: "Produkt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "en", 2 },
                column: "Name",
                value: "Module");

            migrationBuilder.UpdateData(
                table: "ProductTypeTranslation",
                keyColumns: new[] { "Language", "ProductTypeId" },
                keyValues: new object[] { "nb", 2 },
                column: "Name",
                value: "Modul");
        }
    }
}
