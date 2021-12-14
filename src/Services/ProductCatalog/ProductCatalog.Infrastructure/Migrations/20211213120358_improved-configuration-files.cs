using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    public partial class improvedconfigurationfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                table: "ProductFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures");

            migrationBuilder.AlterTable(
                name: "ProductFeatures",
                oldComment: "Join table")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ProductFeaturesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "ProductFeaturesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTypeTranslation",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTypeTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "A short, descriptive name",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductTypeTranslation",
                type: "nvarchar(max)",
                nullable: true,
                comment: "An optional description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "ProductTypeTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format",
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "ProductTypeTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTranslation",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "A short, descriptive name",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: true,
                comment: "An optional description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "ProductTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format",
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The partner that owns and sells this product",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductRequiresOne",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresProductId",
                table: "ProductRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductRequiresAll",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresProductId",
                table: "ProductRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductFeatures",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductFeatures",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "ProductFeatures",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductExcludes",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ExcludesProductId",
                table: "ProductExcludes",
                type: "int",
                nullable: false,
                comment: "The 'ProductId' cant be combined or used with this product. This is a one-way requirement",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductExcludes",
                type: "int",
                nullable: false,
                comment: "The product that has exclusions",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTypeTranslation",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeatureTypeTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "A short, descriptive name",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeatureTypeTranslation",
                type: "nvarchar(max)",
                nullable: true,
                comment: "An optional description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "FeatureTypeTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format",
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureTypeId",
                table: "FeatureTypeTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTypes",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTranslation",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeatureTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                comment: "A short, descriptive name",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeatureTranslation",
                type: "nvarchar(max)",
                nullable: true,
                comment: "An optional description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "FeatureTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                comment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format",
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Features",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureTypeId",
                table: "Features",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "AccessControlPermissionNode",
                table: "Features",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "A fixed and unique access-control node-name that's used by the front- and back-end to enable or disable functionality for an organization. The node-name must consist of only characters, formated in PascalCase. Example: 'MyPermissionNodeIdentifier'",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Features",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureRequiresOne",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresFeatureId",
                table: "FeatureRequiresOne",
                type: "int",
                nullable: false,
                comment: "The 'FeatureID' requires one of these features before it can be added or used. This is a one-way requirement",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureRequiresOne",
                type: "int",
                nullable: false,
                comment: "The feature that has a requirement",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureRequiresAll",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresFeatureId",
                table: "FeatureRequiresAll",
                type: "int",
                nullable: false,
                comment: "The 'FeatureID' requires all these features before it can be added or used. This is a one-way requirement",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureRequiresAll",
                type: "int",
                nullable: false,
                comment: "The feature that has requirements",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureExcludes",
                type: "uniqueidentifier",
                nullable: false,
                comment: "The ID of the user that performed the last update.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ExcludesFeatureId",
                table: "FeatureExcludes",
                type: "int",
                nullable: false,
                comment: "The 'FeatureID' cant be combined or used with this feature. This is a one-way requirement",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureExcludes",
                type: "int",
                nullable: false,
                comment: "The feature that has exclusions",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                table: "ProductFeatures",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                table: "ProductFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures");

            migrationBuilder.AlterTable(
                name: "ProductFeatures",
                comment: "Join table")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ProductFeaturesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "ProductFeaturesHistory")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTypeTranslation",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTypeTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "A short, descriptive name")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductTypeTranslation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "An optional description")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "ProductTypeTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "ProductTypeTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductTranslation",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "A short, descriptive name")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductTranslation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "An optional description")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "ProductTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The partner that owns and sells this product")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductRequiresOne",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresProductId",
                table: "ProductRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductRequiresAll",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresProductId",
                table: "ProductRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductFeatures",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductFeatures",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "ProductFeatures",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "ProductExcludes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "ExcludesProductId",
                table: "ProductExcludes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The 'ProductId' cant be combined or used with this product. This is a one-way requirement")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductExcludes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The product that has exclusions")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTypeTranslation",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeatureTypeTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "A short, descriptive name")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeatureTypeTranslation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "An optional description")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "FeatureTypeTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureTypeId",
                table: "FeatureTypeTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTypes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureTranslation",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeatureTranslation",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldComment: "A short, descriptive name")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeatureTranslation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "An optional description")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "FeatureTranslation",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 2,
                oldComment: "The language for this translation. This should be stored in lowercase 'ISO 639-1' format")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureTranslation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Features",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureTypeId",
                table: "Features",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "AccessControlPermissionNode",
                table: "Features",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "A fixed and unique access-control node-name that's used by the front- and back-end to enable or disable functionality for an organization. The node-name must consist of only characters, formated in PascalCase. Example: 'MyPermissionNodeIdentifier'")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Features",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureRequiresOne",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresFeatureId",
                table: "FeatureRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The 'FeatureID' requires one of these features before it can be added or used. This is a one-way requirement")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureRequiresOne",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The feature that has a requirement")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureRequiresAll",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "RequiresFeatureId",
                table: "FeatureRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The 'FeatureID' requires all these features before it can be added or used. This is a one-way requirement")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureRequiresAll",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The feature that has requirements")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "FeatureExcludes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "The ID of the user that performed the last update.");

            migrationBuilder.AlterColumn<int>(
                name: "ExcludesFeatureId",
                table: "FeatureExcludes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The 'FeatureID' cant be combined or used with this feature. This is a one-way requirement")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "FeatureExcludes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "The feature that has exclusions")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                table: "ProductFeatures",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Products_ProductId",
                table: "ProductFeatures",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
