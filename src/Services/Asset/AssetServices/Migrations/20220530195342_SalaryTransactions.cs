using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class SalaryTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Runtime",
                table: "LifeCycleSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartPeriod",
                table: "AssetLifeCycles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndPeriod",
                table: "AssetLifeCycles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "SalaryDeductionTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<int>(type: "int", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false),
                    AssetLifecycleId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryDeductionTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryDeductionTransaction_AssetLifeCycles_AssetLifecycleId",
                        column: x => x.AssetLifecycleId,
                        principalTable: "AssetLifeCycles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryDeductionTransaction_AssetLifecycleId",
                table: "SalaryDeductionTransaction",
                column: "AssetLifecycleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryDeductionTransaction");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "LifeCycleSettings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartPeriod",
                table: "AssetLifeCycles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndPeriod",
                table: "AssetLifeCycles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
