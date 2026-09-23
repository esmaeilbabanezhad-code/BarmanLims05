using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleToReceptionCorrectionRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SampleId",
                table: "ReceptionCorrectionRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceptionCorrectionRequests_SampleId",
                table: "ReceptionCorrectionRequests",
                column: "SampleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceptionCorrectionRequests_Samples_SampleId",
                table: "ReceptionCorrectionRequests",
                column: "SampleId",
                principalTable: "Samples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceptionCorrectionRequests_Samples_SampleId",
                table: "ReceptionCorrectionRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReceptionCorrectionRequests_SampleId",
                table: "ReceptionCorrectionRequests");

            migrationBuilder.DropColumn(
                name: "SampleId",
                table: "ReceptionCorrectionRequests");
        }
    }
}
