using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class AddedRecycleAndWipeDependency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductRequiresOne",
                columns: new[] { "ProductId", "RequiresProductId", "UpdatedBy" },
                values: new object[] { 5, 3, new Guid("00000000-0000-0000-0000-000000000001") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductRequiresOne",
                keyColumns: new[] { "ProductId", "RequiresProductId" },
                keyValues: new object[] { 5, 3 });
        }
    }
}
