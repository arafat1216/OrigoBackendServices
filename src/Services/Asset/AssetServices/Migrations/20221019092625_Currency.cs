using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffboardBuyoutPrice",
                table: "AssetLifeCycles");

            migrationBuilder.RenameColumn(
                name: "CurrencyCode",
                table: "SalaryDeductionTransaction",
                newName: "Deduction_CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "SalaryDeductionTransaction",
                newName: "Deduction_Amount");

            migrationBuilder.RenameColumn(
                name: "MinBuyoutPrice",
                table: "LifeCycleSetting",
                newName: "MinBuyoutPrice_Amount");

            migrationBuilder.RenameColumn(
                name: "CurrencyCode",
                table: "AssetLifeCycles",
                newName: "PaidByCompany_CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "PaidByCompany",
                table: "AssetLifeCycles",
                newName: "PaidByCompany_Amount");

            migrationBuilder.AlterColumn<string>(
                name: "Deduction_CurrencyCode",
                table: "SalaryDeductionTransaction",
                type: "char(3)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MinBuyoutPrice_CurrencyCode",
                table: "LifeCycleSetting",
                type: "char(3)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaidByCompany_CurrencyCode",
                table: "AssetLifeCycles",
                type: "char(3)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "OffboardBuyoutPrice_Amount",
                table: "AssetLifeCycles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OffboardBuyoutPrice_CurrencyCode",
                table: "AssetLifeCycles",
                type: "char(3)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinBuyoutPrice_CurrencyCode",
                table: "LifeCycleSetting");

            migrationBuilder.DropColumn(
                name: "OffboardBuyoutPrice_Amount",
                table: "AssetLifeCycles");

            migrationBuilder.DropColumn(
                name: "OffboardBuyoutPrice_CurrencyCode",
                table: "AssetLifeCycles");

            migrationBuilder.RenameColumn(
                name: "Deduction_CurrencyCode",
                table: "SalaryDeductionTransaction",
                newName: "CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "Deduction_Amount",
                table: "SalaryDeductionTransaction",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "MinBuyoutPrice_Amount",
                table: "LifeCycleSetting",
                newName: "MinBuyoutPrice");

            migrationBuilder.RenameColumn(
                name: "PaidByCompany_CurrencyCode",
                table: "AssetLifeCycles",
                newName: "CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "PaidByCompany_Amount",
                table: "AssetLifeCycles",
                newName: "PaidByCompany");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyCode",
                table: "SalaryDeductionTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyCode",
                table: "AssetLifeCycles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OffboardBuyoutPrice",
                table: "AssetLifeCycles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
