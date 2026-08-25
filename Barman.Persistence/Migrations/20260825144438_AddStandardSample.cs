using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStandardSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StandardSampleId",
                table: "Samples",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StandardSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardSamples_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StandardSamples_SampleCategories_SampleCategoryId",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_StandardSampleId",
                table: "Samples",
                column: "StandardSampleId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardSamples_MatrixId_Code",
                table: "StandardSamples",
                columns: new[] { "MatrixId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandardSamples_MatrixId_Name",
                table: "StandardSamples",
                columns: new[] { "MatrixId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandardSamples_SampleCategoryId",
                table: "StandardSamples",
                column: "SampleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples",
                column: "StandardSampleId",
                principalTable: "StandardSamples",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_StandardSamples_StandardSampleId",
                table: "Samples");

            migrationBuilder.DropTable(
                name: "StandardSamples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_StandardSampleId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "StandardSampleId",
                table: "Samples");
        }
    }
}
