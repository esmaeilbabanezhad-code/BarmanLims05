using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResultSetItemToTestLimitChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestResultSetItemId",
                table: "TestLimitChangeRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_TestResultSetItemId",
                table: "TestLimitChangeRequests",
                column: "TestResultSetItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitChangeRequests_TestResultSetItems_TestResultSetIte~",
                table: "TestLimitChangeRequests",
                column: "TestResultSetItemId",
                principalTable: "TestResultSetItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitChangeRequests_TestResultSetItems_TestResultSetIte~",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_TestLimitChangeRequests_TestResultSetItemId",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TestResultSetItemId",
                table: "TestLimitChangeRequests");
        }
    }
}
