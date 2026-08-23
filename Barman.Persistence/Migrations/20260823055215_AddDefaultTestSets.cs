using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultTestSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultTestSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_DefaultTestSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultTestSets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultTestSets_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultTestSets_SampleCategories_SampleCategoryId",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefaultTestSetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultTestSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPanel = table.Column<bool>(type: "boolean", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: true),
                    TestPanelId = table.Column<Guid>(type: "uuid", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultTestSetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultTestSetItems_DefaultTestSets_DefaultTestSetId",
                        column: x => x.DefaultTestSetId,
                        principalTable: "DefaultTestSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultTestSetItems_TestPanels_TestPanelId",
                        column: x => x.TestPanelId,
                        principalTable: "TestPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultTestSetItems_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSetItems_DefaultTestSetId_SortOrder",
                table: "DefaultTestSetItems",
                columns: new[] { "DefaultTestSetId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSetItems_DefaultTestSetId_TestId",
                table: "DefaultTestSetItems",
                columns: new[] { "DefaultTestSetId", "TestId" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSetItems_DefaultTestSetId_TestPanelId",
                table: "DefaultTestSetItems",
                columns: new[] { "DefaultTestSetId", "TestPanelId" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSetItems_TestId",
                table: "DefaultTestSetItems",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSetItems_TestPanelId",
                table: "DefaultTestSetItems",
                column: "TestPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSets_Code",
                table: "DefaultTestSets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSets_CustomerId_SampleCategoryId_MatrixId_Priori~",
                table: "DefaultTestSets",
                columns: new[] { "CustomerId", "SampleCategoryId", "MatrixId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSets_MatrixId",
                table: "DefaultTestSets",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultTestSets_SampleCategoryId",
                table: "DefaultTestSets",
                column: "SampleCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultTestSetItems");

            migrationBuilder.DropTable(
                name: "DefaultTestSets");
        }
    }
}
