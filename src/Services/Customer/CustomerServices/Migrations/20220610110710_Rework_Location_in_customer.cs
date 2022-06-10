using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class Rework_Location_in_customer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Locations SET Description='' WHERE Description IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Locations SET Address1='' WHERE Address1 IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Locations SET Address2='' WHERE Address2 IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Locations SET PostalCode='' WHERE PostalCode IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Locations SET City='' WHERE City IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Locations SET Country='' WHERE Country IS NULL");

            migrationBuilder.DropColumn(
                name: "PrimaryLocation",
                table: "Organization");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Locations",
                newName: "ExternalId");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address2",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address1",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecipientType",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_OrganizationId",
                table: "Locations",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Organization_OrganizationId",
                table: "Locations",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Organization_OrganizationId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_OrganizationId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "RecipientType",
                table: "Locations");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Locations",
                newName: "LocationId");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryLocation",
                table: "Organization",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address2",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address1",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
