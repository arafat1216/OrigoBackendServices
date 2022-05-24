using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class makes_modules_mutually_exclusive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductExcludes",
                columns: new[] { "ExcludesProductId", "ProductId", "UpdatedBy" },
                values: new object[] { 3, 2, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "ProductExcludes",
                columns: new[] { "ExcludesProductId", "ProductId", "UpdatedBy" },
                values: new object[] { 2, 3, new Guid("00000000-0000-0000-0000-000000000000") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductExcludes",
                keyColumns: new[] { "ExcludesProductId", "ProductId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ProductExcludes",
                keyColumns: new[] { "ExcludesProductId", "ProductId" },
                keyValues: new object[] { 2, 3 });
        }
    }
}
