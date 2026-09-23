using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestAssignmentToLimitChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestAssignmentId",
                table: "TestLimitChangeRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_TestAssignmentId",
                table: "TestLimitChangeRequests",
                column: "TestAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitChangeRequests_TestAssignments_TestAssignmentId",
                table: "TestLimitChangeRequests",
                column: "TestAssignmentId",
                principalTable: "TestAssignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitChangeRequests_TestAssignments_TestAssignmentId",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_TestLimitChangeRequests_TestAssignmentId",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TestAssignmentId",
                table: "TestLimitChangeRequests");
        }
    }
}
