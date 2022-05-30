using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class updateRoleNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("UPDATE UserPermissions SET RoleId = 8 Where RoleId = 2");
            migrationBuilder.Sql("UPDATE UserPermissions SET RoleId = 9 Where RoleId = 3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE UserPermissions SET RoleId = 2 Where RoleId = 8");
            migrationBuilder.Sql("UPDATE UserPermissions SET RoleId = 3 Where RoleId = 9");
        }
    }
}
