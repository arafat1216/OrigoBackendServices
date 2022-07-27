using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class LastDaySalaryDeduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastDayForReportingSalaryDeduction",
                table: "Organization",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "PayrollContactEmail",
                table: "Organization",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDayForReportingSalaryDeduction",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "PayrollContactEmail",
                table: "Organization");
        }
    }
}
