using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class moduleExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductModuleGroup_ProductModuleGroup_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductModuleGroup_ProductModule_ProductModuleId",
                table: "ProductModuleGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductModuleGroup",
                table: "ProductModuleGroup");

            migrationBuilder.RenameTable(
                name: "ProductModuleGroup",
                newName: "ProductModuleGroups");

            migrationBuilder.RenameIndex(
                name: "IX_ProductModuleGroup_ProductModuleId",
                table: "ProductModuleGroups",
                newName: "IX_ProductModuleGroups_ProductModuleId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductModuleId",
                table: "ProductModule",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductModuleGroupId",
                table: "ProductModuleGroups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductModuleGroups",
                table: "ProductModuleGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductModuleGroup_ProductModuleGroups_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup",
                column: "SelectedProductModuleGroupsId",
                principalTable: "ProductModuleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModuleGroups_ProductModule_ProductModuleId",
                table: "ProductModuleGroups",
                column: "ProductModuleId",
                principalTable: "ProductModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductModuleGroup_ProductModuleGroups_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductModuleGroups_ProductModule_ProductModuleId",
                table: "ProductModuleGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductModuleGroups",
                table: "ProductModuleGroups");

            migrationBuilder.DropColumn(
                name: "ProductModuleId",
                table: "ProductModule");

            migrationBuilder.DropColumn(
                name: "ProductModuleGroupId",
                table: "ProductModuleGroups");

            migrationBuilder.RenameTable(
                name: "ProductModuleGroups",
                newName: "ProductModuleGroup");

            migrationBuilder.RenameIndex(
                name: "IX_ProductModuleGroups_ProductModuleId",
                table: "ProductModuleGroup",
                newName: "IX_ProductModuleGroup_ProductModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductModuleGroup",
                table: "ProductModuleGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductModuleGroup_ProductModuleGroup_SelectedProductModuleGroupsId",
                table: "CustomerProductModuleGroup",
                column: "SelectedProductModuleGroupsId",
                principalTable: "ProductModuleGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModuleGroup_ProductModule_ProductModuleId",
                table: "ProductModuleGroup",
                column: "ProductModuleId",
                principalTable: "ProductModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
