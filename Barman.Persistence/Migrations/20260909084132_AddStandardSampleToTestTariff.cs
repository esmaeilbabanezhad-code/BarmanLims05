using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStandardSampleToTestTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs");

            migrationBuilder.AddColumn<Guid>(
                name: "StandardSampleId",
                table: "TestTariffs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs",
                columns: new[] { "OrganizationTypeId", "TestId", "TestPanelId", "CustomerId", "StandardSampleId", "ValidFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_StandardSampleId",
                table: "TestTariffs",
                column: "StandardSampleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTariffs_StandardSamples_StandardSampleId",
                table: "TestTariffs",
                column: "StandardSampleId",
                principalTable: "StandardSamples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTariffs_StandardSamples_StandardSampleId",
                table: "TestTariffs");

            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs");

            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_StandardSampleId",
                table: "TestTariffs");

            migrationBuilder.DropColumn(
                name: "StandardSampleId",
                table: "TestTariffs");

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs",
                columns: new[] { "OrganizationTypeId", "TestId", "TestPanelId", "CustomerId", "ValidFrom" });
        }
    }
}
