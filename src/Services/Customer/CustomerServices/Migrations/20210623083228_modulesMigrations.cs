using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class modulesMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductModules_ProductModuleGroup_ProductModuleGroupId",
                table: "ProductModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductModules",
                table: "ProductModules");

            migrationBuilder.DropIndex(
                name: "IX_ProductModules_ProductModuleGroupId",
                table: "ProductModules");

            migrationBuilder.DropColumn(
                name: "ProductModuleGroupId",
                table: "ProductModules");

            migrationBuilder.RenameTable(
                name: "ProductModules",
                newName: "ProductModule");

            migrationBuilder.AddColumn<int>(
                name: "ProductModuleId",
                table: "ProductModuleGroup",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductModule",
                table: "ProductModule",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductModuleGroup_ProductModuleId",
                table: "ProductModuleGroup",
                column: "ProductModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModuleGroup_ProductModule_ProductModuleId",
                table: "ProductModuleGroup",
                column: "ProductModuleId",
                principalTable: "ProductModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductModuleGroup_ProductModule_ProductModuleId",
                table: "ProductModuleGroup");

            migrationBuilder.DropIndex(
                name: "IX_ProductModuleGroup_ProductModuleId",
                table: "ProductModuleGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductModule",
                table: "ProductModule");

            migrationBuilder.DropColumn(
                name: "ProductModuleId",
                table: "ProductModuleGroup");

            migrationBuilder.RenameTable(
                name: "ProductModule",
                newName: "ProductModules");

            migrationBuilder.AddColumn<int>(
                name: "ProductModuleGroupId",
                table: "ProductModules",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductModules",
                table: "ProductModules",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductModules_ProductModuleGroupId",
                table: "ProductModules",
                column: "ProductModuleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModules_ProductModuleGroup_ProductModuleGroupId",
                table: "ProductModules",
                column: "ProductModuleGroupId",
                principalTable: "ProductModuleGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
