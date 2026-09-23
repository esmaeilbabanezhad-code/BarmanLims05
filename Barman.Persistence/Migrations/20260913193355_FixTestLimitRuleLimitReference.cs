using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTestLimitRuleLimitReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules");

            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_ReferenceLimits_ReferenceLimitId",
                table: "TestLimitRules");

            migrationBuilder.DropIndex(
                name: "IX_TestLimitRules_ReferenceLimitId",
                table: "TestLimitRules");

            migrationBuilder.DropColumn(
                name: "ReferenceLimitId",
                table: "TestLimitRules");

            migrationBuilder.Sql("""
    UPDATE "TestLimitRules"
    SET "LimitReferenceId" = 'ce79b4a4-c7a3-4cbf-a05f-c704e5182882'
    WHERE "LimitReferenceId" IS NULL;
    """);

            migrationBuilder.AlterColumn<Guid>(
                name: "LimitReferenceId",
                table: "TestLimitRules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId",
                principalTable: "LimitReferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules");

            migrationBuilder.AlterColumn<Guid>(
                name: "LimitReferenceId",
                table: "TestLimitRules",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceLimitId",
                table: "TestLimitRules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_ReferenceLimitId",
                table: "TestLimitRules",
                column: "ReferenceLimitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_LimitReferences_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId",
                principalTable: "LimitReferences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitRules_ReferenceLimits_ReferenceLimitId",
                table: "TestLimitRules",
                column: "ReferenceLimitId",
                principalTable: "ReferenceLimits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
