using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class provides_loan_device : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProvidesLoanDevice",
                table: "CustomerSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvidesLoanDevice",
                table: "CustomerSettings");
        }
    }
}
