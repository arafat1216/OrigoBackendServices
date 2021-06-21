using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class SelectedProductModulesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_ProductModuleGroup_SelectedProductModuleGroupsId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_SelectedProductModuleGroupsId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "SelectedProductModuleGroupsId",
                table: "Customer");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "SelectedProductModuleGroupsId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_SelectedProductModuleGroupsId",
                table: "Customer",
                column: "SelectedProductModuleGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_ProductModuleGroup_SelectedProductModuleGroupsId",
                table: "Customer",
                column: "SelectedProductModuleGroupsId",
                principalTable: "ProductModuleGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
