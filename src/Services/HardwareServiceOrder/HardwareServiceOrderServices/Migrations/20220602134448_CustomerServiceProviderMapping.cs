using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class CustomerServiceProviderMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryAddress_RecipientType",
                table: "HardwareServiceOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    AssetCategoryId = table.Column<int>(type: "int", nullable: false),
                    ApiUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateFetched = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerServiceProvider", x => new { x.CustomerId, x.Id, x.AssetCategoryId, x.ServiceProviderId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerServiceProvider");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress_RecipientType",
                table: "HardwareServiceOrders");
        }
    }
}
