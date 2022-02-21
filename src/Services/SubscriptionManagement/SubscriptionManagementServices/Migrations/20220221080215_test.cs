using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                table: "CustomerReferenceField");

            migrationBuilder.DropForeignKey(
                name: "FK_DataPackage_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                table: "DataPackage");

            migrationBuilder.DropIndex(
                name: "IX_DataPackage_CustomerSubscriptionProductId",
                table: "DataPackage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerReferenceField",
                table: "CustomerReferenceField");

            migrationBuilder.DropColumn(
                name: "CustomerSubscriptionProductId",
                table: "DataPackage");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerSettingsId",
                table: "CustomerReferenceField",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerReferenceField",
                table: "CustomerReferenceField",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CustomersDatapackage",
                columns: table => new
                {
                    CustomerSubscriptionProductsId = table.Column<int>(type: "int", nullable: false),
                    DataPackagesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersDatapackage", x => new { x.CustomerSubscriptionProductsId, x.DataPackagesId });
                    table.ForeignKey(
                        name: "FK_CustomersDatapackage_CustomerSubscriptionProduct_CustomerSubscriptionProductsId",
                        column: x => x.CustomerSubscriptionProductsId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersDatapackage_DataPackage_DataPackagesId",
                        column: x => x.DataPackagesId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReferenceField_CustomerSettingsId",
                table: "CustomerReferenceField",
                column: "CustomerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersDatapackage_DataPackagesId",
                table: "CustomersDatapackage",
                column: "DataPackagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                table: "CustomerReferenceField",
                column: "CustomerSettingsId",
                principalTable: "CustomerSettings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                table: "CustomerReferenceField");

            migrationBuilder.DropTable(
                name: "CustomersDatapackage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerReferenceField",
                table: "CustomerReferenceField");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReferenceField_CustomerSettingsId",
                table: "CustomerReferenceField");

            migrationBuilder.AddColumn<int>(
                name: "CustomerSubscriptionProductId",
                table: "DataPackage",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerSettingsId",
                table: "CustomerReferenceField",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerReferenceField",
                table: "CustomerReferenceField",
                columns: new[] { "CustomerSettingsId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_DataPackage_CustomerSubscriptionProductId",
                table: "DataPackage",
                column: "CustomerSubscriptionProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                table: "CustomerReferenceField",
                column: "CustomerSettingsId",
                principalTable: "CustomerSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DataPackage_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                table: "DataPackage",
                column: "CustomerSubscriptionProductId",
                principalTable: "CustomerSubscriptionProduct",
                principalColumn: "Id");
        }
    }
}
