using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalManagerRoutingToTestAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TechnicalManagerId",
                table: "TestAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TechnicalManagerScopeId",
                table: "TestAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignments_TechnicalManagerId",
                table: "TestAssignments",
                column: "TechnicalManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignments_TechnicalManagerScopeId",
                table: "TestAssignments",
                column: "TechnicalManagerScopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAssignments_Employees_TechnicalManagerId",
                table: "TestAssignments",
                column: "TechnicalManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAssignments_TechnicalManagerScopes_TechnicalManagerScop~",
                table: "TestAssignments",
                column: "TechnicalManagerScopeId",
                principalTable: "TechnicalManagerScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAssignments_Employees_TechnicalManagerId",
                table: "TestAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAssignments_TechnicalManagerScopes_TechnicalManagerScop~",
                table: "TestAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TestAssignments_TechnicalManagerId",
                table: "TestAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TestAssignments_TechnicalManagerScopeId",
                table: "TestAssignments");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerId",
                table: "TestAssignments");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerScopeId",
                table: "TestAssignments");
        }
    }
}
