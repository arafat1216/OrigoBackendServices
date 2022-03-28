using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServices.Migrations
{
    public partial class database_value_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Changes the old non-nullable default value to the new nullable default value.
            migrationBuilder.Sql(@"
                    UPDATE Organization
                    SET DeletedBy = null
                    WHERE DeletedBy = '00000000-0000-0000-0000-000000000000' AND IsDeleted = 0;
            ");

            // Changes the old non-nullable default value to the new nullable default value.
            migrationBuilder.Sql(@"
                UPDATE Organization
                SET ParentId = null
                WHERE ParentId = '00000000-0000-0000-0000-000000000000';
            ");

            // Changes the old non-nullable default value to the new nullable default value.
            migrationBuilder.Sql(@"
                UPDATE Organization
                SET PrimaryLocation = null
                WHERE PrimaryLocation = '00000000-0000-0000-0000-000000000000';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
