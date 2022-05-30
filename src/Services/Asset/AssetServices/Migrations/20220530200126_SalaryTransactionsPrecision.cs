using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class SalaryTransactionsPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "SalaryDeductionTransaction",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SalaryDeductionTransaction",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "SalaryDeductionTransaction",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SalaryDeductionTransaction",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");
        }
    }
}
