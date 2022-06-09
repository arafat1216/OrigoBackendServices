using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class added_service_events : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ServiceEvent",
                columns: table => new
                {
                    HardwareServiceOrderId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceStatusId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "When this event was recorded in the external service-provider's system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEvent", x => new { x.HardwareServiceOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServiceEvent_HardwareServiceOrders_HardwareServiceOrderId",
                        column: x => x.HardwareServiceOrderId,
                        principalTable: "HardwareServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_CustomerId",
                table: "HardwareServiceOrders",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceEvent");

            migrationBuilder.DropIndex(
                name: "IX_HardwareServiceOrders_CustomerId",
                table: "HardwareServiceOrders");

        }
    }
}
