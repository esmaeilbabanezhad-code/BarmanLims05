using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportTemplateDesignerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "ReportTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefault",
                table: "ReportTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemTemplate",
                table: "ReportTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "ReportTemplateFields",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alignment",
                table: "ReportTemplateFields",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "ReportTemplateFields",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ReportTemplateFields",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_IsSystemDefault",
                table: "ReportTemplates",
                column: "IsSystemDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportTemplates_IsSystemDefault",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IsSystemDefault",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IsSystemTemplate",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "Alignment",
                table: "ReportTemplateFields");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ReportTemplateFields");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ReportTemplateFields");

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "ReportTemplateFields",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
