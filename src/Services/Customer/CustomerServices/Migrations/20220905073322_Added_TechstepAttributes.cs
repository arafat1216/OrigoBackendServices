using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class Added_TechstepAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechstepAccountOwner",
                table: "Organization",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TechstepCustomerId",
                table: "Organization",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechstepAccountOwner",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "TechstepCustomerId",
                table: "Organization");
        }
    }
}
