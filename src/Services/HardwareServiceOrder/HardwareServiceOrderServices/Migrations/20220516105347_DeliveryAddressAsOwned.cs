using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class DeliveryAddressAsOwned : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HardwareServiceOrders_DeliveryAddresses_DeliveryAddressId",
                table: "HardwareServiceOrders");

            migrationBuilder.DropTable(
                name: "DeliveryAddresses");

            migrationBuilder.DropIndex(
                name: "IX_HardwareServiceOrders_DeliveryAddressId",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                table: "HardwareServiceOrders");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_Address1",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_Address2",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_City",
                table: "HardwareServiceOrders",
                type: "nvarchar(85)",
                maxLength: 85,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_Country",
                table: "HardwareServiceOrders",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: true,
                comment: "The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard.");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_PostalCode",
                table: "HardwareServiceOrders",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress_Recipient",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAddress_Address1",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_Address2",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_City",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_Country",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_PostalCode",
                table: "HardwareServiceOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_Recipient",
                table: "HardwareServiceOrders");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryAddressId",
                table: "HardwareServiceOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(85)", maxLength: 85, nullable: false),
                    Country = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false, comment: "The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard."),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    PostalCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_DeliveryAddressId",
                table: "HardwareServiceOrders",
                column: "DeliveryAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_HardwareServiceOrders_DeliveryAddresses_DeliveryAddressId",
                table: "HardwareServiceOrders",
                column: "DeliveryAddressId",
                principalTable: "DeliveryAddresses",
                principalColumn: "Id");
        }
    }
}
