using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestResultSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: true),
                    StandardSampleId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResultSets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultSets_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultSets_SampleCategories_SampleCategoryId",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultSets_StandardSamples_StandardSampleId",
                        column: x => x.StandardSampleId,
                        principalTable: "StandardSamples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultSets_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResultSetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestResultSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestResultDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultSetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResultSetItems_TestResultDefinitions_TestResultDefiniti~",
                        column: x => x.TestResultDefinitionId,
                        principalTable: "TestResultDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultSetItems_TestResultSets_TestResultSetId",
                        column: x => x.TestResultSetId,
                        principalTable: "TestResultSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSetItems_TestResultDefinitionId",
                table: "TestResultSetItems",
                column: "TestResultDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSetItems_TestResultSetId_DisplayOrder",
                table: "TestResultSetItems",
                columns: new[] { "TestResultSetId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSetItems_TestResultSetId_TestResultDefinitionId",
                table: "TestResultSetItems",
                columns: new[] { "TestResultSetId", "TestResultDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_Code",
                table: "TestResultSets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_CustomerId",
                table: "TestResultSets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_MatrixId",
                table: "TestResultSets",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_SampleCategoryId",
                table: "TestResultSets",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_StandardSampleId",
                table: "TestResultSets",
                column: "StandardSampleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultSets_TestId_CustomerId_SampleCategoryId_MatrixId_~",
                table: "TestResultSets",
                columns: new[] { "TestId", "CustomerId", "SampleCategoryId", "MatrixId", "StandardSampleId", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResultSetItems");

            migrationBuilder.DropTable(
                name: "TestResultSets");
        }
    }
}
