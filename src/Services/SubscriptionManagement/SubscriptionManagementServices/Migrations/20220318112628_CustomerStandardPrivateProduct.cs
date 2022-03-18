using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class CustomerStandardPrivateProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerStandardPrivateSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPackage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerSubscriptionProductId = table.Column<int>(type: "int", nullable: true),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerStandardPrivateSubscriptionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerStandardPrivateSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerStandardPrivateSubscriptionProduct_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                        column: x => x.CustomerSubscriptionProductId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStandardPrivateSubscriptionProduct_CustomerOperatorSettingsId",
                table: "CustomerStandardPrivateSubscriptionProduct",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStandardPrivateSubscriptionProduct_CustomerSubscriptionProductId",
                table: "CustomerStandardPrivateSubscriptionProduct",
                column: "CustomerSubscriptionProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerStandardPrivateSubscriptionProduct");
        }
    }
}
