using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetServices.Migrations
{
    public partial class RemovedIsActiveFieldFromAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "IsActive",
            //    table: "Asset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsActive",
            //    table: "Asset",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);
        }
    }
}
