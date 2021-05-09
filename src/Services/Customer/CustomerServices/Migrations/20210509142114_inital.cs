using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerServices.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress_PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerContactPerson_FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerContactPerson_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerContactPerson_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
