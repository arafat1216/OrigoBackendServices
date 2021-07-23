using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class Lifecycles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetCategoryLifecycleTypeCustomer");

            migrationBuilder.DropTable(
                name: "AssetCategoryTypeCustomer");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AssetCategory");

            migrationBuilder.RenameColumn(
                name: "AssetCategoryLifecycleId",
                table: "AssetCategoryLifecycleType",
                newName: "CustomerId");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetCategoryId",
                table: "AssetCategoryLifecycleType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "AssetCategoryLifecycleType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LifecycleType",
                table: "AssetCategoryLifecycleType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "AssetCategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalCustomerId",
                table: "AssetCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryLifecycleType_CustomerId1",
                table: "AssetCategoryLifecycleType",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategory_CustomerId",
                table: "AssetCategory",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategoryLifecycleType_Customer_CustomerId1",
                table: "AssetCategoryLifecycleType",
                column: "CustomerId1",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategory_Customer_CustomerId",
                table: "AssetCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategoryLifecycleType_Customer_CustomerId1",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategoryLifecycleType_CustomerId1",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategory_CustomerId",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "LifecycleType",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "AssetCategory");

            migrationBuilder.DropColumn(
                name: "ExternalCustomerId",
                table: "AssetCategory");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "AssetCategoryLifecycleType",
                newName: "AssetCategoryLifecycleId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AssetCategoryLifecycleType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AssetCategory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetCategoryLifecycleTypeCustomer",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedAssetCategoryLifecyclesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategoryLifecycleTypeCustomer", x => new { x.CustomersId, x.SelectedAssetCategoryLifecyclesId });
                    table.ForeignKey(
                        name: "FK_AssetCategoryLifecycleTypeCustomer_AssetCategoryLifecycleType_SelectedAssetCategoryLifecyclesId",
                        column: x => x.SelectedAssetCategoryLifecyclesId,
                        principalTable: "AssetCategoryLifecycleType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetCategoryLifecycleTypeCustomer_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetCategoryTypeCustomer",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    SelectedAssetCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategoryTypeCustomer", x => new { x.CustomersId, x.SelectedAssetCategoriesId });
                    table.ForeignKey(
                        name: "FK_AssetCategoryTypeCustomer_AssetCategory_SelectedAssetCategoriesId",
                        column: x => x.SelectedAssetCategoriesId,
                        principalTable: "AssetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetCategoryTypeCustomer_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryLifecycleTypeCustomer_SelectedAssetCategoryLifecyclesId",
                table: "AssetCategoryLifecycleTypeCustomer",
                column: "SelectedAssetCategoryLifecyclesId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryTypeCustomer_SelectedAssetCategoriesId",
                table: "AssetCategoryTypeCustomer",
                column: "SelectedAssetCategoriesId");
        }
    }
}
