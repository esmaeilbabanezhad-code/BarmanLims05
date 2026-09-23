using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTestLimitRuleReferenceLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_LimitReference_LimitReferenceId",
                table: "TestLimitRules");



            migrationBuilder.RenameColumn(
                name: "LimitReferenceId",
                table: "TestLimitRules",
                newName: "ReferenceLimitId");

            migrationBuilder.RenameIndex(
                name: "IX_TestLimitRules_LimitReferenceId",
                table: "TestLimitRules",
                newName: "IX_TestLimitRules_ReferenceLimitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_ReferenceLimits_ReferenceLimitId",
                table: "TestLimitRules",
                column: "ReferenceLimitId",
                principalTable: "ReferenceLimits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_ReferenceLimits_ReferenceLimitId",
                table: "TestLimitRules");

            migrationBuilder.RenameColumn(
                name: "ReferenceLimitId",
                table: "TestLimitRules",
                newName: "LimitReferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_TestLimitRules_ReferenceLimitId",
                table: "TestLimitRules",
                newName: "IX_TestLimitRules_LimitReferenceId");

            migrationBuilder.CreateTable(
                name: "LimitReference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DocumentNo = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitReference", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_LimitReference_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId",
                principalTable: "LimitReference",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
