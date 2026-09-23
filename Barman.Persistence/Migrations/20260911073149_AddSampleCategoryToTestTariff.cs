using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleCategoryToTestTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs");

            migrationBuilder.AddColumn<Guid>(
                name: "SampleCategoryId",
                table: "TestTariffs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs",
                columns: new[] { "OrganizationTypeId", "TestId", "TestPanelId", "CustomerId", "SampleCategoryId", "StandardSampleId", "ValidFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_SampleCategoryId",
                table: "TestTariffs",
                column: "SampleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTariffs_SampleCategories_SampleCategoryId",
                table: "TestTariffs",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTariffs_SampleCategories_SampleCategoryId",
                table: "TestTariffs");

            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs");

            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_SampleCategoryId",
                table: "TestTariffs");

            migrationBuilder.DropColumn(
                name: "SampleCategoryId",
                table: "TestTariffs");

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_OrganizationTypeId_TestId_TestPanelId_CustomerI~",
                table: "TestTariffs",
                columns: new[] { "OrganizationTypeId", "TestId", "TestPanelId", "CustomerId", "StandardSampleId", "ValidFrom" });
        }
    }
}
