using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultTestPanelToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultTestPanelId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultTestPanelId",
                table: "Customers",
                column: "DefaultTestPanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_TestPanels_DefaultTestPanelId",
                table: "Customers",
                column: "DefaultTestPanelId",
                principalTable: "TestPanels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_TestPanels_DefaultTestPanelId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DefaultTestPanelId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultTestPanelId",
                table: "Customers");
        }
    }
}
