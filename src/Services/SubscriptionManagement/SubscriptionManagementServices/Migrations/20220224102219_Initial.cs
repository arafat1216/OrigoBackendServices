using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class Initial : Migration
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
                name: "SubscriptionAddOnProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddOnProductName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "CustomerReferenceField",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    CustomerSettingsId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReferenceField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReferenceField_CustomerSettings_CustomerSettingsId",
                        column: x => x.CustomerSettingsId,
                        principalTable: "CustomerSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerOperatorAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "CustomerSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    GlobalSubscriptionProductId = table.Column<int>(type: "int", nullable: true),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSubscriptionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_CustomerOperatorSettings_CustomerOperatorSettingsId",
                        column: x => x.CustomerOperatorSettingsId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_Operator_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptionProduct_SubscriptionProduct_GlobalSubscriptionProductId",
                        column: x => x.GlobalSubscriptionProductId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataPackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPackageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_DataPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataPackage_SubscriptionProduct_SubscriptionProductId",
                        column: x => x.SubscriptionProductId,
                        principalTable: "SubscriptionProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomersDatapackage",
                columns: table => new
                {
                    CustomerSubscriptionProductsId = table.Column<int>(type: "int", nullable: false),
                    DataPackagesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersDatapackage", x => new { x.CustomerSubscriptionProductsId, x.DataPackagesId });
                    table.ForeignKey(
                        name: "FK_CustomersDatapackage_CustomerSubscriptionProduct_CustomerSubscriptionProductsId",
                        column: x => x.CustomerSubscriptionProductsId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersDatapackage_DataPackage_DataPackagesId",
                        column: x => x.DataPackagesId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateToBusinessSubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SIMCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorAccountId = table.Column<int>(type: "int", nullable: false),
                    DataPackageId = table.Column<int>(type: "int", nullable: false),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateToBusinessSubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateToBusinessSubscriptionOrder_CustomerOperatorAccount_OperatorAccountId",
                        column: x => x.OperatorAccountId,
                        principalTable: "CustomerOperatorAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateToBusinessSubscriptionOrder_CustomerSubscriptionProduct_SubscriptionProductId",
                        column: x => x.SubscriptionProductId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateToBusinessSubscriptionOrder_DataPackage_DataPackageId",
                        column: x => x.DataPackageId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SIMCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorAccountId = table.Column<int>(type: "int", nullable: false),
                    DataPackageId = table.Column<int>(type: "int", nullable: false),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_SubscriptionProductId",
                        column: x => x.SubscriptionProductId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                        column: x => x.DataPackageId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrivateToBusinessSubscriptionOrderAddOnProducts",
                columns: table => new
                {
                    PrivateToBusinessSubscriptionOrdersId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateToBusinessSubscriptionOrderAddOnProducts", x => new { x.PrivateToBusinessSubscriptionOrdersId, x.SubscriptionAddOnProductsId });
                    table.ForeignKey(
                        name: "FK_PrivateToBusinessSubscriptionOrderAddOnProducts_PrivateToBusinessSubscriptionOrder_PrivateToBusinessSubscriptionOrdersId",
                        column: x => x.PrivateToBusinessSubscriptionOrdersId,
                        principalTable: "PrivateToBusinessSubscriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateToBusinessSubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionOrderAddOnProducts",
                columns: table => new
                {
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionOrderAddOnProducts", x => new { x.SubscriptionAddOnProductsId, x.SubscriptionOrdersId });
                    table.ForeignKey(
                        name: "FK_SubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrderAddOnProducts_SubscriptionOrder_SubscriptionOrdersId",
                        column: x => x.SubscriptionOrdersId,
                        principalTable: "SubscriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_CustomerOperatorAccount_OrganizationId_AccountNumber",
                table: "CustomerOperatorAccount",
                columns: new[] { "OrganizationId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_CustomerSettingsId",
                table: "CustomerOperatorSettings",
                column: "CustomerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOperatorSettings_OperatorId",
                table: "CustomerOperatorSettings",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReferenceField_CustomerSettingsId",
                table: "CustomerReferenceField",
                column: "CustomerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersDatapackage_DataPackagesId",
                table: "CustomersDatapackage",
                column: "DataPackagesId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersOperatorAccounts_CustomerOperatorSettingsId",
                table: "CustomersOperatorAccounts",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptionProduct_CustomerOperatorSettingsId",
                table: "CustomerSubscriptionProduct",
                column: "CustomerOperatorSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptionProduct_GlobalSubscriptionProductId",
                table: "CustomerSubscriptionProduct",
                column: "GlobalSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptionProduct_OperatorId",
                table: "CustomerSubscriptionProduct",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DataPackage_SubscriptionProductId",
                table: "DataPackage",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateToBusinessSubscriptionOrder_DataPackageId",
                table: "PrivateToBusinessSubscriptionOrder",
                column: "DataPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateToBusinessSubscriptionOrder_OperatorAccountId",
                table: "PrivateToBusinessSubscriptionOrder",
                column: "OperatorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateToBusinessSubscriptionOrder_SubscriptionProductId",
                table: "PrivateToBusinessSubscriptionOrder",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateToBusinessSubscriptionOrderAddOnProducts_SubscriptionAddOnProductsId",
                table: "PrivateToBusinessSubscriptionOrderAddOnProducts",
                column: "SubscriptionAddOnProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_DataPackageId",
                table: "SubscriptionOrder",
                column: "DataPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_SubscriptionProductId",
                table: "SubscriptionOrder",
                column: "SubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrderAddOnProducts_SubscriptionOrdersId",
                table: "SubscriptionOrderAddOnProducts",
                column: "SubscriptionOrdersId");

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
                name: "CustomersDatapackage");

            migrationBuilder.DropTable(
                name: "CustomersOperatorAccounts");

            migrationBuilder.DropTable(
                name: "PrivateToBusinessSubscriptionOrderAddOnProducts");

            migrationBuilder.DropTable(
                name: "SubscriptionOrderAddOnProducts");

            migrationBuilder.DropTable(
                name: "PrivateToBusinessSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "SubscriptionOrder");

            migrationBuilder.DropTable(
                name: "CustomerOperatorAccount");

            migrationBuilder.DropTable(
                name: "CustomerSubscriptionProduct");

            migrationBuilder.DropTable(
                name: "DataPackage");

            migrationBuilder.DropTable(
                name: "CustomerOperatorSettings");

            migrationBuilder.DropTable(
                name: "SubscriptionProduct");

            migrationBuilder.DropTable(
                name: "CustomerSettings");

            migrationBuilder.DropTable(
                name: "Operator");
        }
    }
}
