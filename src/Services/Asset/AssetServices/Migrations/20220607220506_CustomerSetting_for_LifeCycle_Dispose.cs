using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class CustomerSetting_for_LifeCycle_Dispose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LifeCycleSettings",
                table: "LifeCycleSettings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LifeCycleSettings");

            migrationBuilder.RenameTable(
                name: "LifeCycleSettings",
                newName: "LifeCycleSetting");

            migrationBuilder.AddColumn<int>(
                name: "CustomerSettingsId",
                table: "LifeCycleSetting",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LifeCycleSetting",
                table: "LifeCycleSetting",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DisposeSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposeSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisposeSettingId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSettings_DisposeSetting_DisposeSettingId",
                        column: x => x.DisposeSettingId,
                        principalTable: "DisposeSetting",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeCycleSetting_CustomerSettingsId",
                table: "LifeCycleSetting",
                column: "CustomerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSettings_DisposeSettingId",
                table: "CustomerSettings",
                column: "DisposeSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_LifeCycleSetting_CustomerSettings_CustomerSettingsId",
                table: "LifeCycleSetting",
                column: "CustomerSettingsId",
                principalTable: "CustomerSettings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LifeCycleSetting_CustomerSettings_CustomerSettingsId",
                table: "LifeCycleSetting");

            migrationBuilder.DropTable(
                name: "CustomerSettings");

            migrationBuilder.DropTable(
                name: "DisposeSetting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LifeCycleSetting",
                table: "LifeCycleSetting");

            migrationBuilder.DropIndex(
                name: "IX_LifeCycleSetting_CustomerSettingsId",
                table: "LifeCycleSetting");

            migrationBuilder.DropColumn(
                name: "CustomerSettingsId",
                table: "LifeCycleSetting");

            migrationBuilder.RenameTable(
                name: "LifeCycleSetting",
                newName: "LifeCycleSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "LifeCycleSettings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LifeCycleSettings",
                table: "LifeCycleSettings",
                column: "Id");
        }
    }
}
