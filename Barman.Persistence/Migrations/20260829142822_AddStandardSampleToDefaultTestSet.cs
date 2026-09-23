using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStandardSampleToDefaultTestSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StandardSampleId",
                table: "DefaultTestSets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSets_StandardSampleId",
                table: "DefaultTestSets",
                column: "StandardSampleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultTestSets_StandardSamples_StandardSampleId",
                table: "DefaultTestSets",
                column: "StandardSampleId",
                principalTable: "StandardSamples",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultTestSets_StandardSamples_StandardSampleId",
                table: "DefaultTestSets");

            migrationBuilder.DropIndex(
                name: "IX_DefaultTestSets_StandardSampleId",
                table: "DefaultTestSets");

            migrationBuilder.DropColumn(
                name: "StandardSampleId",
                table: "DefaultTestSets");
        }
    }
}
