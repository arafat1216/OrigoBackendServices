using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class CreatedByUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderedBy_Email",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderedBy_ExternalId",
                table: "HardwareServiceOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrderedBy_Name",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderedBy_Email",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "OrderedBy_ExternalId",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "OrderedBy_Name",
                table: "HardwareServiceOrders");
        }
    }
}
