using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class bookValue_change_in_seedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow book value.", "Book Value" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Tilgjengeliggjør bokført verdi.", "Bokført verdi" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow book value and Purchase Price.", "Book Value and Purchase Price" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Tilgjengeliggjør bokført verdi og kjøpspris.", "Bokført verdi og kjøpspris" });
        }
    }
}
