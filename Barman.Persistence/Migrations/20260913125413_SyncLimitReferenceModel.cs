using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncLimitReferenceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LimitReferenceId",
                table: "TestLimitRules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LimitReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentNo = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId",
                principalTable: "LimitReferences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules");

            migrationBuilder.DropTable(
                name: "LimitReferences");

            migrationBuilder.DropIndex(
                name: "IX_TestLimitRules_LimitReferenceId",
                table: "TestLimitRules");

            migrationBuilder.DropColumn(
                name: "LimitReferenceId",
                table: "TestLimitRules");
        }
    }
}
