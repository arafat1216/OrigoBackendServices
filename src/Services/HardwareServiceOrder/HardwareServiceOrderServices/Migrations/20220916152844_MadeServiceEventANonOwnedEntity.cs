using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class MadeServiceEventANonOwnedEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ServiceEvents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "ServiceEvents",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateDeleted",
                table: "ServiceEvents",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "ServiceEvents",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ServiceEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ServiceEvents",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceEvents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ServiceEvents");
        }
    }
}
