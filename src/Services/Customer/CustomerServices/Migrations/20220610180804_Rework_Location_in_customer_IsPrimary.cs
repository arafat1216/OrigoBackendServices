using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class Rework_Location_in_customer_IsPrimary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Locations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Locations");
        }
    }
}
