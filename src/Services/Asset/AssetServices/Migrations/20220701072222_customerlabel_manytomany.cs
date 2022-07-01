using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class customerlabel_manytomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLabels_AssetLifeCycles_AssetLifecycleId",
                table: "CustomerLabels");

            migrationBuilder.DropIndex(
                name: "IX_CustomerLabels_AssetLifecycleId",
                table: "CustomerLabels");

            migrationBuilder.DropColumn(
                name: "AssetLifecycleId",
                table: "CustomerLabels");

            migrationBuilder.CreateTable(
                name: "AssetLifecycleCustomerLabel",
                columns: table => new
                {
                    AsssetLifecyclesId = table.Column<int>(type: "int", nullable: false),
                    LabelsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLifecycleCustomerLabel", x => new { x.AsssetLifecyclesId, x.LabelsId });
                    table.ForeignKey(
                        name: "FK_AssetLifecycleCustomerLabel_AssetLifeCycles_AsssetLifecyclesId",
                        column: x => x.AsssetLifecyclesId,
                        principalTable: "AssetLifeCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetLifecycleCustomerLabel_CustomerLabels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "CustomerLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetLifecycleCustomerLabel_LabelsId",
                table: "AssetLifecycleCustomerLabel",
                column: "LabelsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetLifecycleCustomerLabel");

            migrationBuilder.AddColumn<int>(
                name: "AssetLifecycleId",
                table: "CustomerLabels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLabels_AssetLifecycleId",
                table: "CustomerLabels",
                column: "AssetLifecycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLabels_AssetLifeCycles_AssetLifecycleId",
                table: "CustomerLabels",
                column: "AssetLifecycleId",
                principalTable: "AssetLifeCycles",
                principalColumn: "Id");
        }
    }
}
