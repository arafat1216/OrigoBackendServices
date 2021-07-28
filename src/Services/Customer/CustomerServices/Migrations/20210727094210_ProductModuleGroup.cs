using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class ProductModuleGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CustomerProductModule",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModulesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductModule", x => new { x.CustomersId, x.SelectedProductModulesId });
                    table.ForeignKey(
                        name: "FK_CustomerProductModule_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerProductModule_ProductModule_SelectedProductModulesId",
                        column: x => x.SelectedProductModulesId,
                        principalTable: "ProductModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModule_SelectedProductModulesId",
                table: "CustomerProductModule",
                column: "SelectedProductModulesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerProductModule");

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
    }
}
