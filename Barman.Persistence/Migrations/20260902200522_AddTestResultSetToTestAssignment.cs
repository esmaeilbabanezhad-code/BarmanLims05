using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultSetToTestAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestResultSetId",
                table: "TestAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignments_TestResultSetId",
                table: "TestAssignments",
                column: "TestResultSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAssignments_TestResultSets_TestResultSetId",
                table: "TestAssignments",
                column: "TestResultSetId",
                principalTable: "TestResultSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAssignments_TestResultSets_TestResultSetId",
                table: "TestAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TestAssignments_TestResultSetId",
                table: "TestAssignments");

            migrationBuilder.DropColumn(
                name: "TestResultSetId",
                table: "TestAssignments");
        }
    }
}
