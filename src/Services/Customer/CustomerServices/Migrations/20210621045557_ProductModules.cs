using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class ProductModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedProductModuleGroupsId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductModuleGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModuleGroup", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_ProductModuleGroup_SelectedProductModuleGroupsId",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "ProductModuleGroup");

            migrationBuilder.DropIndex(
                name: "IX_Customer_SelectedProductModuleGroupsId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "SelectedProductModuleGroupsId",
                table: "Customer");
        }
    }
}
