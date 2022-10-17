using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class language_string_limit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "UserPreference",
                type: "varchar(2)",
                unicode: false,
                maxLength: 2,
                nullable: true,
                comment: "A Language using ISO-634 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryLanguage",
                table: "OrganizationPreferences",
                type: "varchar(2)",
                unicode: false,
                maxLength: 2,
                nullable: false,
                comment: "A Language using ISO-634 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "UserPreference",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldUnicode: false,
                oldMaxLength: 2,
                oldNullable: true,
                oldComment: "A Language using ISO-634 format.");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryLanguage",
                table: "OrganizationPreferences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldUnicode: false,
                oldMaxLength: 2,
                oldComment: "A Language using ISO-634 format.");
        }
    }
}
