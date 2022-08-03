using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class ServiceProviderExtentions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterTable(
                name: "CustomerServiceProvider",
                comment: "Configures a customer's service-provider settings.");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ServiceProviders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ServiceOrderAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000000, 1"),
                    ThirdPartyId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "The ID that is used in the external service provider's systems."),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    IsUserSelectable = table.Column<bool>(type: "bit", nullable: false),
                    IsCustomerTogglable = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ServiceOrderAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrderAddons_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The service-order addons that is offered by a given service-provider.");

            migrationBuilder.CreateTable(
                name: "ServiceProviderServiceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000000, 1"),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ServiceProviderServiceType", x => x.Id);
                    table.UniqueConstraint("AK_ServiceProviderServiceType_ServiceProviderId_ServiceTypeId", x => new { x.ServiceProviderId, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_ServiceProviderServiceType_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProviderServiceType_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Determines what service-types is available for a given service-provider.");

            migrationBuilder.InsertData(
                table: "ServiceOrderAddons",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsCustomerTogglable", "IsDeleted", "IsUserSelectable", "ServiceProviderId", "ThirdPartyId", "UpdatedBy" },
                values: new object[] { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, true, false, true, 1, "", new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ServiceProviderServiceType",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "ServiceProviderId", "ServiceTypeId", "UpdatedBy" },
                values: new object[] { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, 1, 3, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Conmodo (NO)");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderAddons_ServiceProviderId",
                table: "ServiceOrderAddons",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderServiceType_ServiceTypeId",
                table: "ServiceProviderServiceType",
                column: "ServiceTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOrderAddons");

            migrationBuilder.DropTable(
                name: "ServiceProviderServiceType");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ServiceProviders");

            migrationBuilder.AlterTable(
                name: "CustomerServiceProvider",
                oldComment: "Configures a customer's service-provider settings.");

            migrationBuilder.InsertData(
                table: "ServiceProviders",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "OrganizationId", "UpdatedBy" },
                values: new object[] { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000001") });
        }
    }
}
