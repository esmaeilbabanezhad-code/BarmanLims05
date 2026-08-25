using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixStandardSampleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples",
                column: "StandardSampleId",
                principalTable: "StandardSamples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples",
                column: "StandardSampleId",
                principalTable: "StandardSamples",
                principalColumn: "Id");
        }
    }
}
