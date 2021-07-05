using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class CustomerModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductModuleGroup_Customer_CustomerId",
                table: "ProductModuleGroup");

            migrationBuilder.DropIndex(
                name: "IX_ProductModuleGroup_CustomerId",
                table: "ProductModuleGroup");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductModuleGroup");

            migrationBuilder.CreateTable(
                name: "CustomerProductModuleGroup",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedProductModuleGroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductModuleGroup", x => new { x.CustomersId, x.SelectedProductModuleGroupsId });
                    table.ForeignKey(
                        name: "FK_CustomerProductModuleGroup_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerProductModuleGroup_ProductModuleGroup_SelectedProductModuleGroupsId",
                        column: x => x.SelectedProductModuleGroupsId,
                        principalTable: "ProductModuleGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductModuleGroup_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup",
                column: "SelectedProductModuleGroupsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerProductModuleGroup");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ProductModuleGroup",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductModuleGroup_CustomerId",
                table: "ProductModuleGroup",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModuleGroup_Customer_CustomerId",
                table: "ProductModuleGroup",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
