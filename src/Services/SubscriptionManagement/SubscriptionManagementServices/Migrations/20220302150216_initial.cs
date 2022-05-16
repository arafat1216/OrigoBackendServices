using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSubscription", x => x.Id);
                });

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
                name: "PrivateSubscription",
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
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RealOwnerId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateSubscription_PrivateSubscription_RealOwnerId",
                        column: x => x.RealOwnerId,
                        principalTable: "PrivateSubscription",
                        principalColumn: "Id");
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
                name: "TransferToBusinessSubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorAccountOrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SimCardNumber = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    SIMCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataPackageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorAccountOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorAccountPayer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrivateSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    BusinessSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    SubscriptionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferToBusinessSubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferToBusinessSubscriptionOrder_BusinessSubscription_BusinessSubscriptionId",
                        column: x => x.BusinessSubscriptionId,
                        principalTable: "BusinessSubscription",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferToBusinessSubscriptionOrder_PrivateSubscription_PrivateSubscriptionId",
                        column: x => x.PrivateSubscriptionId,
                        principalTable: "PrivateSubscription",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransferToPrivateSubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserInfoId = table.Column<int>(type: "int", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewSubscription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferToPrivateSubscriptionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferToPrivateSubscriptionOrder_PrivateSubscription_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "PrivateSubscription",
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
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    ConnectedOrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOperatorSettingId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_CustomerOperatorAccount_CustomerOperatorSettings_CustomerOperatorSettingId",
                        column: x => x.CustomerOperatorSettingId,
                        principalTable: "CustomerOperatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOperatorAccount_Operator_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSubscriptionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    CustomerOperatorSettingsId = table.Column<int>(type: "int", nullable: false),
                    GlobalSubscriptionProductId = table.Column<int>(type: "int", nullable: true),
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
                name: "TransferToBusinessSubscriptionOrderAddOnProducts",
                columns: table => new
                {
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false),
                    TransferToBusinessSubscriptionOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferToBusinessSubscriptionOrderAddOnProducts", x => new { x.SubscriptionAddOnProductsId, x.TransferToBusinessSubscriptionOrdersId });
                    table.ForeignKey(
                        name: "FK_TransferToBusinessSubscriptionOrderAddOnProducts_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferToBusinessSubscriptionOrderAddOnProducts_TransferToBusinessSubscriptionOrder_TransferToBusinessSubscriptionOrdersId",
                        column: x => x.TransferToBusinessSubscriptionOrdersId,
                        principalTable: "TransferToBusinessSubscriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "SubscriptionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerSubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    SimCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SIMCardAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionProductId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorAccountId = table.Column<int>(type: "int", nullable: true),
                    DataPackageId = table.Column<int>(type: "int", nullable: false),
                    OrderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerReferenceFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_CustomerSubscriptionProduct_CustomerSubscriptionProductId",
                        column: x => x.CustomerSubscriptionProductId,
                        principalTable: "CustomerSubscriptionProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionOrder_DataPackage_DataPackageId",
                        column: x => x.DataPackageId,
                        principalTable: "DataPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAddOnProductSubscriptionOrder",
                columns: table => new
                {
                    SubscriptionAddOnProductsId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAddOnProductSubscriptionOrder", x => new { x.SubscriptionAddOnProductsId, x.SubscriptionOrdersId });
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionAddOnProduct_SubscriptionAddOnProductsId",
                        column: x => x.SubscriptionAddOnProductsId,
                        principalTable: "SubscriptionAddOnProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrder_SubscriptionOrdersId",
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
                name: "IX_CustomerOperatorAccount_CustomerOperatorSettingId",
                table: "CustomerOperatorAccount",
                column: "CustomerOperatorSettingId");

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
                name: "IX_PrivateSubscription_RealOwnerId",
                table: "PrivateSubscription",
                column: "RealOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddOnProductSubscriptionOrder_SubscriptionOrdersId",
                table: "SubscriptionAddOnProductSubscriptionOrder",
                column: "SubscriptionOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_CustomerSubscriptionProductId",
                table: "SubscriptionOrder",
                column: "CustomerSubscriptionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_DataPackageId",
                table: "SubscriptionOrder",
                column: "DataPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionOrder_OperatorAccountId",
                table: "SubscriptionOrder",
                column: "OperatorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionProduct_OperatorId",
                table: "SubscriptionProduct",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferToBusinessSubscriptionOrder_BusinessSubscriptionId",
                table: "TransferToBusinessSubscriptionOrder",
                column: "BusinessSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferToBusinessSubscriptionOrder_PrivateSubscriptionId",
                table: "TransferToBusinessSubscriptionOrder",
                column: "PrivateSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferToBusinessSubscriptionOrderAddOnProducts_TransferToBusinessSubscriptionOrdersId",
                table: "TransferToBusinessSubscriptionOrderAddOnProducts",
                column: "TransferToBusinessSubscriptionOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferToPrivateSubscriptionOrder_UserInfoId",
                table: "TransferToPrivateSubscriptionOrder",
                column: "UserInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReferenceField");

            migrationBuilder.DropTable(
                name: "CustomersDatapackage");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProductSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "TransferToBusinessSubscriptionOrderAddOnProducts");

            migrationBuilder.DropTable(
                name: "TransferToPrivateSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "SubscriptionOrder");

            migrationBuilder.DropTable(
                name: "SubscriptionAddOnProduct");

            migrationBuilder.DropTable(
                name: "TransferToBusinessSubscriptionOrder");

            migrationBuilder.DropTable(
                name: "CustomerOperatorAccount");

            migrationBuilder.DropTable(
                name: "CustomerSubscriptionProduct");

            migrationBuilder.DropTable(
                name: "DataPackage");

            migrationBuilder.DropTable(
                name: "BusinessSubscription");

            migrationBuilder.DropTable(
                name: "PrivateSubscription");

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
