using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    LoanDevicePhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true, comment: "A phone-number using E.164 format."),
                    LoanDeviceEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProvidesLoanDevice = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CustomerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ServiceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
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
                    table.PrimaryKey("PK_ServiceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
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
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    ApiUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateFetched = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
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
                    table.PrimaryKey("PK_CustomerServiceProvider", x => x.Id);
                    table.UniqueConstraint("AK_CustomerServiceProvider_CustomerId_ServiceProviderId", x => new { x.CustomerId, x.ServiceProviderId });
                    table.ForeignKey(
                        name: "FK_CustomerServiceProvider_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HardwareServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetLifecycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetLifecycleCategoryId = table.Column<int>(type: "int", nullable: false),
                    UserDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalServiceManagementLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceProviderOrderId1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceProviderOrderId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReturnLoanDeviceEmailSent = table.Column<bool>(type: "bit", nullable: false),
                    IsOrderDiscardedEmailSent = table.Column<bool>(type: "bit", nullable: false),
                    IsOrderCancellationEmailSent = table.Column<bool>(type: "bit", nullable: false),
                    Owner_UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Owner_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner_Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Owner_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DeliveryAddress_RecipientType = table.Column<int>(type: "int", nullable: true),
                    DeliveryAddress_Recipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress_PostalCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    DeliveryAddress_City = table.Column<string>(type: "nvarchar(85)", maxLength: 85, nullable: true),
                    DeliveryAddress_Country = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true, comment: "The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard."),
                    AssetInfo_Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetInfo_Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetInfo_Imei = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetInfo_SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetInfo_PurchaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    AssetInfo_Accessories = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedAssetInfo_Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedAssetInfo_Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedAssetInfo_Imei = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedAssetInfo_SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnedAssetInfo_PurchaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    ReturnedAssetInfo_Accessories = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_HardwareServiceOrders", x => x.Id);
                    table.UniqueConstraint("AK_HardwareServiceOrders_ExternalId", x => x.ExternalId);
                    table.ForeignKey(
                        name: "FK_HardwareServiceOrders_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HardwareServiceOrders_ServiceStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ServiceStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HardwareServiceOrders_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    ServiceStatusId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "When this event was recorded in the external service-provider's system"),
                    HardwareServiceOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEvents_HardwareServiceOrders_HardwareServiceOrderId",
                        column: x => x.HardwareServiceOrderId,
                        principalTable: "HardwareServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ServiceProviders",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "OrganizationId", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ServiceStatuses",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 16, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "CreatedBy", "DateDeleted", "DateUpdated", "DeletedBy", "IsDeleted", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, false, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerServiceProvider_ServiceProviderId",
                table: "CustomerServiceProvider",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSettings_CustomerId",
                table: "CustomerSettings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_AssetLifecycleId",
                table: "HardwareServiceOrders",
                column: "AssetLifecycleId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_CustomerId",
                table: "HardwareServiceOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_Owner_UserId",
                table: "HardwareServiceOrders",
                column: "Owner_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_ServiceProviderId",
                table: "HardwareServiceOrders",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_ServiceTypeId",
                table: "HardwareServiceOrders",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HardwareServiceOrders_StatusId",
                table: "HardwareServiceOrders",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvents_HardwareServiceOrderId",
                table: "ServiceEvents",
                column: "HardwareServiceOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerServiceProvider");

            migrationBuilder.DropTable(
                name: "CustomerSettings");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "ServiceEvents");

            migrationBuilder.DropTable(
                name: "HardwareServiceOrders");

            migrationBuilder.DropTable(
                name: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "ServiceStatuses");

            migrationBuilder.DropTable(
                name: "ServiceTypes");
        }
    }
}
