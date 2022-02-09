using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReferenceField",
                columns: table => new
                {
                    CustomerSettingsId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReferenceField", x => new { x.CustomerSettingsId, x.Id });
                    table.ForeignKey(
                        name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                        column: x => x.CustomerSettingsId,
                        principalTable: "CustomerSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOperatorAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOperatorAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOperatorAccount_Operator_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOperatorSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    CustomerSettingsId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOperatorSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOperatorSettings_CustomerSettings_CustomerSettingsId",
                        column: x => x.CustomerSettingsId,
                        principalTable: "CustomerSettings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerOperatorSettings_Operator_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionProduct_Operator_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomersOperatorAccounts",
                columns: table => new
                {
                    CustomerOperatorAccountsId = table.Column<int>(type: "int", nullable: false),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersOperatorAccounts", x => new { x.CustomerOperatorAccountsId, x.CustomerOperatorSettingsId });
                    table.ForeignKey(
                        name: "FK_CustomersOperatorAccounts_CustomerOperatorAccount_CustomerOperatorAccountsId",
                        column: x => x.CustomerOperatorAccountsId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersOperatorAccounts_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomersOperatorSubscriptionProduct",
                columns: table => new
                {
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersOperatorSubscriptionProduct", x => new { x.CustomerOperatorSettingsId, x.SubscriptionProductsId });
                    table.ForeignKey(
                        name: "FK_CustomersOperatorSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersOperatorSubscriptionProduct_SubscriptionProduct_SubscriptionProductsId",
                        column: x => x.SubscriptionProductsId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Datapackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatapackageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datapackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Datapackage_SubscriptionProduct_SubscriptionProductId",
                        column: x => x.SubscriptionProductId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorAccountId = table.Column<int>(type: "int", nullable: false),
                    DatapackageId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                        column: x => x.OperatorAccountId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_Datapackage_DatapackageId",
                        column: x => x.DatapackageId,
                        principalTable: "Datapackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_SubscriptionProduct_SubscriptionProductId",
                        column: x => x.SubscriptionProductId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAddOnProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddOnProductName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubscriptionOrderId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAddOnProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOnProduct_SubscriptionOrder_SubscriptionOrderId",
                        column: x => x.SubscriptionOrderId,
                        principalTable: "SubscriptionOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Operator",
                columns: new[] { "Id", "Country", "CreatedBy", "CreatedDate", "DeletedBy", "IsDeleted", "OperatorName", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telia - SE", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, "nb", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - NO", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, "se", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381), new Guid("00000000-0000-0000-0000-000000000000"), false, "Telenor - SE", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorAccount_OperatorId",
                table: "CustomerOperatorAccount",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_CustomerSettingsId",
                table: "CustomerOperatorSettings",
                column: "CustomerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_OperatorId",
                table: "CustomerOperatorSettings",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersOperatorAccounts_CustomerOperatorSettingsId",
                table: "CustomersOperatorAccounts",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersOperatorSubscriptionProduct_SubscriptionProductsId",
                table: "CustomersOperatorSubscriptionProduct",
                column: "SubscriptionProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Datapackage_SubscriptionProductId",
                table: "Datapackage",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOnProduct_SubscriptionOrderId",
                table: "SubscriptionAddOnProduct",
                column: "SubscriptionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_DatapackageId",
                table: "SubscriptionOrder",
                column: "DatapackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_SubscriptionProductId",
                table: "SubscriptionOrder",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionProduct_OperatorId",
                table: "SubscriptionProduct",
                column: "OperatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReferenceField");

            migrationBuilder.DropTable(
                name: "CustomersOperatorAccounts");

            migrationBuilder.DropTable(
                name: "CustomersOperatorSubscriptionProduct");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "CustomerOperatorSettings");

            migrationBuilder.DropTable(
                name: "SubscriptionOrder");

            migrationBuilder.DropTable(
                name: "CustomerSettings");

            migrationBuilder.DropTable(
                name: "CustomerOperatorAccount");

            migrationBuilder.DropTable(
                name: "Datapackage");

            migrationBuilder.DropTable(
                name: "SubscriptionProduct");

            migrationBuilder.DropTable(
                name: "Operator");
        }
    }
}
