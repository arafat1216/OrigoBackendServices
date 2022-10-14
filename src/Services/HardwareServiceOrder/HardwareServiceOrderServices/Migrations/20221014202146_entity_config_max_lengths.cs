using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    public partial class entity_config_max_lengths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReturnedAssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "A JSON-serialized list that contains all the device's IMEI numbers. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReturnedAssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "A JSON-serialized list detailing what accessories, if any, was included in the shipment. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Owner_PhoneNumber",
                table: "HardwareServiceOrders",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: true,
                comment: "A phone-number using E.164 format.",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "A JSON-serialized list that contains all the device's IMEI numbers.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "A JSON-serialized list detailing what accessories, if any, was included in the shipment.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReturnedAssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "A JSON-serialized list that contains all the device's IMEI numbers. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.");

            migrationBuilder.AlterColumn<string>(
                name: "ReturnedAssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "A JSON-serialized list detailing what accessories, if any, was included in the shipment. In most cases this will be null, as we generally only record the value if it's provided, and the value has changed.");

            migrationBuilder.AlterColumn<string>(
                name: "Owner_PhoneNumber",
                table: "HardwareServiceOrders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15,
                oldNullable: true,
                oldComment: "A phone-number using E.164 format.");

            migrationBuilder.AlterColumn<string>(
                name: "AssetInfo_Imei",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "A JSON-serialized list that contains all the device's IMEI numbers.");

            migrationBuilder.AlterColumn<string>(
                name: "AssetInfo_Accessories",
                table: "HardwareServiceOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "A JSON-serialized list detailing what accessories, if any, was included in the shipment.");
        }
    }
}
