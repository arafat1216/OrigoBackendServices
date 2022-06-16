using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetServices.Migrations
{
    public partial class UnnecessaryExternalId_in_DisposeSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisposeSetting_ExternalId",
                table: "CustomerSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DisposeSetting_ExternalId",
                table: "CustomerSettings",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
