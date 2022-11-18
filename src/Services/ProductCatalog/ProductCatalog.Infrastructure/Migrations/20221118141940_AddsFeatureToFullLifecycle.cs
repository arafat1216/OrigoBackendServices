using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class AddsFeatureToFullLifecycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.InsertData(
                table: "ProductFeatures",
                columns: new[] { "FeatureId", "ProductId", "UpdatedBy" },
                values: new object[] { 15, 3, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 3 },
                column: "Name",
                value: "Full Lifecycle");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 3 },
                column: "Name",
                value: "Full Lifecycle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductFeatures",
                keyColumns: new[] { "FeatureId", "ProductId" },
                keyValues: new object[] { 15, 3 });

            migrationBuilder.InsertData(
                table: "ProductRequiresOne",
                columns: new[] { "ProductId", "RequiresProductId", "UpdatedBy" },
                values: new object[] { 4, 3, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "en", 3 },
                column: "Name",
                value: "Transactional Device Lifecycle Management");

            migrationBuilder.UpdateData(
                table: "ProductTranslation",
                keyColumns: new[] { "Language", "ProductId" },
                keyValues: new object[] { "nb", 3 },
                column: "Name",
                value: "Transactional Device Lifecycle Management");
        }
    }
}
