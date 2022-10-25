using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class default_runtime_value : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Updating Existing Runtime values
            migrationBuilder.Sql("UPDATE [dbo].[LifeCycleSetting] SET Runtime = 36 where Runtime < 2");

            migrationBuilder.AlterColumn<int>(
                name: "Runtime",
                table: "LifeCycleSetting",
                type: "int",
                nullable: true,
                defaultValue: 36,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Runtime",
                table: "LifeCycleSetting",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 36);
        }
    }
}
