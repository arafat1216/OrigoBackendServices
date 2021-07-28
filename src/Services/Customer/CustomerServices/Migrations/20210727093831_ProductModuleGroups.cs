using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class ProductModuleGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ProductModule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductModule_CustomerId",
                table: "ProductModule",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModule_Customer_CustomerId",
                table: "ProductModule",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductModule_Customer_CustomerId",
                table: "ProductModule");

            migrationBuilder.DropIndex(
                name: "IX_ProductModule_CustomerId",
                table: "ProductModule");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductModule");
        }
    }
}
