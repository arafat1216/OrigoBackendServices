using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class CustomerStandardSubscriptionPrivateProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerStandardPrivateSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPackage = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings",
                column: "StandardPrivateSubscriptionProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOperatorSettings_CustomerStandardPrivateSubscriptionProduct_StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings",
                column: "StandardPrivateSubscriptionProductId",
                principalTable: "CustomerStandardPrivateSubscriptionProduct",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOperatorSettings_CustomerStandardPrivateSubscriptionProduct_StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings");

            migrationBuilder.DropTable(
                name: "CustomerStandardPrivateSubscriptionProduct");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOperatorSettings_StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings");

            migrationBuilder.DropColumn(
                name: "StandardPrivateSubscriptionProductId",
                table: "CustomerOperatorSettings");
        }
    }
}
