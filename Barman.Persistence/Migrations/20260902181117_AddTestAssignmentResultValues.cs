using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestAssignmentResultValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestAssignmentResultValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestResultSetItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAssignmentResultValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAssignmentResultValues_TestAssignments_TestAssignmentId",
                        column: x => x.TestAssignmentId,
                        principalTable: "TestAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestAssignmentResultValues_TestResultSetItems_TestResultSet~",
                        column: x => x.TestResultSetItemId,
                        principalTable: "TestResultSetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignmentResultValues_TestAssignmentId_TestResultSetIt~",
                table: "TestAssignmentResultValues",
                columns: new[] { "TestAssignmentId", "TestResultSetItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignmentResultValues_TestResultSetItemId",
                table: "TestAssignmentResultValues",
                column: "TestResultSetItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestAssignmentResultValues");
        }
    }
}
