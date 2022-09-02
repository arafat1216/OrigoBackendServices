using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class AddedCustomerServiceProviderServiceOrderAddons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerServiceProviderServiceOrderAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000000, 1"),
                    CustomerServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    ServiceOrderAddonId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerServiceProviderServiceOrderAddons", x => x.Id);
                    table.UniqueConstraint("AK_CustomerServiceProviderServiceOrderAddons_CustomerServiceProviderId_ServiceOrderAddonId", x => new { x.CustomerServiceProviderId, x.ServiceOrderAddonId });
                    table.ForeignKey(
                        name: "FK_CustomerServiceProviderServiceOrderAddons_CustomerServiceProvider_CustomerServiceProviderId",
                        column: x => x.CustomerServiceProviderId,
                        principalTable: "CustomerServiceProvider",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerServiceProviderServiceOrderAddons_ServiceOrderAddons_ServiceOrderAddonId",
                        column: x => x.ServiceOrderAddonId,
                        principalTable: "ServiceOrderAddons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerServiceProviderServiceOrderAddons_ServiceOrderAddonId",
                table: "CustomerServiceProviderServiceOrderAddons",
                column: "ServiceOrderAddonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerServiceProviderServiceOrderAddons");
        }
    }
}
