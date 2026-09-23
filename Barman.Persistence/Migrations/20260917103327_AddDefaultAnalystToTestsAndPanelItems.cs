using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAnalystToTestsAndPanelItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultAnalystId",
                table: "Tests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultAnalystId",
                table: "TestPanelItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_DefaultAnalystId",
                table: "Tests",
                column: "DefaultAnalystId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPanelItems_DefaultAnalystId",
                table: "TestPanelItems",
                column: "DefaultAnalystId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPanelItems_Employees_DefaultAnalystId",
                table: "TestPanelItems",
                column: "DefaultAnalystId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Employees_DefaultAnalystId",
                table: "Tests",
                column: "DefaultAnalystId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPanelItems_Employees_DefaultAnalystId",
                table: "TestPanelItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Employees_DefaultAnalystId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_DefaultAnalystId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_TestPanelItems_DefaultAnalystId",
                table: "TestPanelItems");

            migrationBuilder.DropColumn(
                name: "DefaultAnalystId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "DefaultAnalystId",
                table: "TestPanelItems");
        }
    }
}
