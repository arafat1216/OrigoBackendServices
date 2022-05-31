using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class parterId_in_SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "nb" },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Gir organisasjonen mulighet til å utføre oppgaver relatert til bokført verdi og kjøpspris", "Håndtering av Bokført verdi" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Tilgjengeliggjør bokført verdi og kjøpspris", "Bokført verdi og kjøpspris" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "PartnerId",
                value: new Guid("5741b4a1-4eef-4fc2-b1b8-0ba7f41ed93c"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "PartnerId",
                value: new Guid("5741b4a1-4eef-4fc2-b1b8-0ba7f41ed93c"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "PartnerId",
                value: new Guid("5741b4a1-4eef-4fc2-b1b8-0ba7f41ed93c"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "PartnerId",
                value: new Guid("5741b4a1-4eef-4fc2-b1b8-0ba7f41ed93c"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FeatureTranslation",
                keyColumns: new[] { "FeatureId", "Language" },
                keyValues: new object[] { 5, "nb" },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Gir kunder tilgang til grunnleggende administrering av ikke-personlige assets.", "Grunnleggende administrering av ikke-personlige assets" });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 4 },
                columns: new[] { "Description", "Name" },
                values: new object[] { "Ett partner spesifikk abonnement-håndtering produkt", "Abonnement-håndtering" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "PartnerId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
