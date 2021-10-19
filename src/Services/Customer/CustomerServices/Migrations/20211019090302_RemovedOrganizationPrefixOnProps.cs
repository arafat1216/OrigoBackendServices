using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class RemovedOrganizationPrefixOnProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_PhoneNumber",
                table: "Customer",
                newName: "ContactPerson_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_FullName",
                table: "Customer",
                newName: "ContactPerson_FullName");

            migrationBuilder.RenameColumn(
                name: "OrganizationContactPerson_Email",
                table: "Customer",
                newName: "ContactPerson_Email");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_Street",
                table: "Customer",
                newName: "Address_Street");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_PostCode",
                table: "Customer",
                newName: "Address_PostCode");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_Country",
                table: "Customer",
                newName: "Address_Country");

            migrationBuilder.RenameColumn(
                name: "OrganizationAddress_City",
                table: "Customer",
                newName: "Address_City");

            migrationBuilder.RenameColumn(
                name: "OrganizationName",
                table: "Customer",
                newName: "Name");    

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(7033));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(3302));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4346));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 752, DateTimeKind.Utc).AddTicks(4347));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(8576));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9091));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9095));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9096));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9097));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 10, 19, 9, 3, 1, 753, DateTimeKind.Utc).AddTicks(9098));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactPerson_PhoneNumber",
                table: "Customer",
                newName: "OrganizationContactPerson_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "ContactPerson_FullName",
                table: "Customer",
                newName: "OrganizationContactPerson_FullName");

            migrationBuilder.RenameColumn(
                name: "ContactPerson_Email",
                table: "Customer",
                newName: "OrganizationContactPerson_Email");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                table: "Customer",
                newName: "OrganizationAddress_Street");

            migrationBuilder.RenameColumn(
                name: "Address_PostCode",
                table: "Customer",
                newName: "OrganizationAddress_PostCode");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "Customer",
                newName: "OrganizationAddress_Country");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "Customer",
                newName: "OrganizationAddress_City");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customer",
                newName: "OrganizationName");       

            migrationBuilder.UpdateData(
                table: "PermissionSets",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(2845));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(3760));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4473));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4475));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 594, DateTimeKind.Utc).AddTicks(4476));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(3841));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4187));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4190));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4191));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2021, 9, 16, 8, 6, 43, 595, DateTimeKind.Utc).AddTicks(4193));       
        }
    }
}
