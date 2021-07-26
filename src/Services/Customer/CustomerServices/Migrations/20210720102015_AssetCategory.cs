using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class AssetCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "LifecycleType",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "AssetCategoryLifecycleType",
                newName: "AssetCategoryLifecycleId");

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AssetCategoryLifecycleType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategory", x => x.Id);
                });

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
                name: "IX_AssetCategoryLifecycleType_AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType",
                column: "AssetCategoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryLifecycleTypeCustomer_SelectedAssetCategoryLifecyclesId",
                table: "AssetCategoryLifecycleTypeCustomer",
                column: "SelectedAssetCategoryLifecyclesId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategoryTypeCustomer_SelectedAssetCategoriesId",
                table: "AssetCategoryTypeCustomer",
                column: "SelectedAssetCategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategoryLifecycleType_AssetCategory_AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType",
                column: "AssetCategoryTypeId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategoryLifecycleType_AssetCategory_AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropTable(
                name: "AssetCategoryLifecycleTypeCustomer");

            migrationBuilder.DropTable(
                name: "AssetCategoryTypeCustomer");

            migrationBuilder.DropTable(
                name: "AssetCategory");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategoryLifecycleType_AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "AssetCategoryTypeId",
                table: "AssetCategoryLifecycleType");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AssetCategoryLifecycleType");

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
                name: "LifecycleType",
                table: "AssetCategoryLifecycleType",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
