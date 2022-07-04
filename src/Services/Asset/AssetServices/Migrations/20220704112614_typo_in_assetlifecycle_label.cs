using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class typo_in_assetlifecycle_label : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLifecycleCustomerLabel_AssetLifeCycles_AsssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel");

            migrationBuilder.RenameColumn(
                name: "AsssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel",
                newName: "AssetLifecyclesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLifecycleCustomerLabel_AssetLifeCycles_AssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel",
                column: "AssetLifecyclesId",
                principalTable: "AssetLifeCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLifecycleCustomerLabel_AssetLifeCycles_AssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel");

            migrationBuilder.RenameColumn(
                name: "AssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel",
                newName: "AsssetLifecyclesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLifecycleCustomerLabel_AssetLifeCycles_AsssetLifecyclesId",
                table: "AssetLifecycleCustomerLabel",
                column: "AsssetLifecyclesId",
                principalTable: "AssetLifeCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
