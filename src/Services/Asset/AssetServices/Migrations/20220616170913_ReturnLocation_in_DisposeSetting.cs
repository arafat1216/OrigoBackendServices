using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class ReturnLocation_in_DisposeSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSettings_DisposeSetting_DisposeSettingId",
                table: "CustomerSettings");

            migrationBuilder.DropTable(
                name: "DisposeSetting");

            migrationBuilder.DropIndex(
                name: "IX_CustomerSettings_DisposeSettingId",
                table: "CustomerSettings");

            migrationBuilder.RenameColumn(
                name: "DisposeSettingId",
                table: "CustomerSettings",
                newName: "DisposeSetting_Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "DisposeSetting_CreatedBy",
                table: "CustomerSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisposeSetting_CreatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisposeSetting_DeletedBy",
                table: "CustomerSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisposeSetting_ExternalId",
                table: "CustomerSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DisposeSetting_IsDeleted",
                table: "CustomerSettings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisposeSetting_LastUpdatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisposeSetting_PayrollContactEmail",
                table: "CustomerSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisposeSetting_UpdatedBy",
                table: "CustomerSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReturnLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisposeSettingCustomerSettingsId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnLocation", x => new { x.DisposeSettingCustomerSettingsId, x.Id });
                    table.ForeignKey(
                        name: "FK_ReturnLocation_CustomerSettings_DisposeSettingCustomerSettingsId",
                        column: x => x.DisposeSettingCustomerSettingsId,
                        principalTable: "CustomerSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnLocation");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_CreatedBy",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_CreatedDate",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_DeletedBy",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_ExternalId",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_IsDeleted",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_LastUpdatedDate",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_PayrollContactEmail",
                table: "CustomerSettings");

            migrationBuilder.DropColumn(
                name: "DisposeSetting_UpdatedBy",
                table: "CustomerSettings");

            migrationBuilder.RenameColumn(
                name: "DisposeSetting_Id",
                table: "CustomerSettings",
                newName: "DisposeSettingId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "CustomerSettings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.CreateTable(
                name: "DisposeSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    PayrollContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposeSetting", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSettings_DisposeSettingId",
                table: "CustomerSettings",
                column: "DisposeSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSettings_DisposeSetting_DisposeSettingId",
                table: "CustomerSettings",
                column: "DisposeSettingId",
                principalTable: "DisposeSetting",
                principalColumn: "Id");
        }
    }
}
